import { useEffect, useState } from 'react'
import { api, thongBaoLoi } from './auth.jsx'
import { fmtNgay, fmtGio, homNay } from './format.js'

const TEN_GIOI = { Nam: 'Nam', Nu: 'Nữ', Khac: 'Khác' }
const CLS_LICH = { ChoTiem: 'warn', DaTiem: 'ok', DaHuy: 'bad' }
const CLS_CCD = { ChuaXacNhan: 'warn', DaXacNhan: 'bad', DaThuHoi: '' }

// Trang dành cho người dân: chỉ thấy và thao tác trên hồ sơ của chính mình
export default function CuaToiPage() {
  const [tt, setTt] = useState(null)
  const [vaccines, setVaccines] = useState([])
  const [lich, setLich] = useState([])
  const [mui, setMui] = useState([])
  const [ccd, setCcd] = useState([])
  const [form, setForm] = useState({ vaccineId: '', muiThu: '1', ngayHen: '' })
  const [msg, setMsg] = useState(null)

  const loadLich = async () => setLich(await api('/api/cua-toi/lich'))

  useEffect(() => {
    Promise.all([
      api('/api/cua-toi/thong-tin'), api('/api/vaccine'), api('/api/cua-toi/lich'),
      api('/api/cua-toi/mui-tiem'), api('/api/cua-toi/chong-chi-dinh'),
    ])
      .then(([a, b, c, d, e]) => { setTt(a); setVaccines(b); setLich(c); setMui(d); setCcd(e) })
      .catch((e) => setMsg({ type: 'err', text: thongBaoLoi(e) }))
  }, [])

  const vc = vaccines.find((v) => String(v.id) === form.vaccineId)
  const set = (k) => (e) => setForm({ ...form, [k]: e.target.value })

  async function datLich(e) {
    e.preventDefault()
    setMsg(null)
    try {
      await api('/api/cua-toi/lich', {
        method: 'POST',
        body: { vaccineId: Number(form.vaccineId), muiThu: Number(form.muiThu), ngayHen: form.ngayHen },
      })
      setMsg({ type: 'ok', text: 'Đã đặt lịch tiêm.' })
      setForm({ ...form, muiThu: '1', ngayHen: '' })
      await loadLich()
    } catch (ex) {
      setMsg({ type: 'err', text: thongBaoLoi(ex) })
    }
  }

  async function huy(l) {
    if (!window.confirm(`Hủy lịch tiêm ${l.vaccine} mũi ${l.muiThu} ngày ${fmtNgay(l.ngayHen)}?`)) return
    setMsg(null)
    try {
      await api(`/api/cua-toi/lich/${l.id}/huy`, { method: 'POST', body: { lyDo: 'Người tiêm tự hủy lịch' } })
      setMsg({ type: 'ok', text: 'Đã hủy lịch.' })
      await loadLich()
    } catch (ex) {
      setMsg({ type: 'err', text: thongBaoLoi(ex) })
    }
  }

  return (
    <>
      <h1>Trang của tôi</h1>
      {msg && msg.type === 'err' && !tt && <p className="msg err" style={{ marginLeft: 0 }}>{msg.text}</p>}

      {tt && (
        <div className="card">
          <h2>Thông tin cá nhân</h2>
          <div className="stats" style={{ marginBottom: 0 }}>
            <div className="stat"><span>Họ và tên</span><b style={{ fontSize: 18 }}>{tt.hoTen}</b></div>
            <div className="stat"><span>Ngày sinh</span><b style={{ fontSize: 18 }}>{fmtNgay(tt.ngaySinh)}</b></div>
            <div className="stat"><span>Giới tính</span><b style={{ fontSize: 18 }}>{TEN_GIOI[tt.gioiTinh] ?? 'Chưa rõ'}</b></div>
            <div className="stat"><span>Số điện thoại</span><b style={{ fontSize: 18 }}>{tt.soDienThoai ?? ''}</b></div>
            <div className="stat"><span>Email</span><b style={{ fontSize: 18 }}>{tt.email ?? ''}</b></div>
          </div>
        </div>
      )}

      <form className="card" onSubmit={datLich}>
        <h2>Đặt lịch tiêm</h2>
        <div className="row">
          <label>Vaccine
            <select value={form.vaccineId} onChange={(e) => setForm({ ...form, vaccineId: e.target.value, muiThu: '1' })} required>
              <option value="">Chọn...</option>
              {vaccines.map((v) => <option key={v.id} value={v.id}>{v.ten} ({v.soMui} mũi)</option>)}
            </select>
          </label>
          <label>Mũi thứ
            <select value={form.muiThu} onChange={set('muiThu')} disabled={!vc}>
              {Array.from({ length: vc?.soMui ?? 1 }, (_, i) => i + 1).map((n) => <option key={n} value={n}>Mũi {n}</option>)}
            </select>
          </label>
          <label>Ngày hẹn
            <input type="date" value={form.ngayHen} onChange={set('ngayHen')} min={homNay()} required />
          </label>
        </div>
        {vc?.khoangCachNgay != null && Number(form.muiThu) > 1 &&
          <p className="muted">Mũi {form.muiThu} phải cách mũi trước ít nhất {vc.khoangCachNgay} ngày.</p>}
        <button type="submit">Đặt lịch</button>
        {msg && tt && <span className={'msg ' + msg.type}>{msg.text}</span>}
      </form>

      <div className="card">
        <h2>Lịch hẹn của tôi ({lich.length})</h2>
        <div className="tw">
          <table>
            <thead><tr><th>Vaccine</th><th>Mũi</th><th>Ngày hẹn</th><th>Trạng thái</th><th></th></tr></thead>
            <tbody>
              {lich.map((l) => (
                <tr key={l.id}>
                  <td>{l.vaccine}</td><td>{l.muiThu}</td><td>{fmtNgay(l.ngayHen)}</td>
                  <td><span className={'tag ' + CLS_LICH[l.trangThai]}>{l.tenTrangThai}</span></td>
                  <td>{l.trangThai === 'ChoTiem' && <button className="danger sm" onClick={() => huy(l)}>Hủy lịch</button>}</td>
                </tr>
              ))}
              {lich.length === 0 && <tr><td colSpan="5">Bạn chưa có lịch hẹn nào.</td></tr>}
            </tbody>
          </table>
        </div>
      </div>

      <div className="card">
        <h2>Lịch sử tiêm ({mui.length})</h2>
        <div className="tw">
          <table>
            <thead><tr><th>Thời điểm</th><th>Vaccine</th><th>Mũi</th><th>Số lô</th><th>Người tiêm</th></tr></thead>
            <tbody>
              {mui.map((m) => (
                <tr key={m.muiTiemId}>
                  <td>{fmtGio(m.ngayGioTiem)}</td><td>{m.vaccine}</td><td>{m.muiThu}</td><td>{m.soLo}</td><td>{m.nhanVien}</td>
                </tr>
              ))}
              {mui.length === 0 && <tr><td colSpan="5">Bạn chưa tiêm mũi nào tại đây.</td></tr>}
            </tbody>
          </table>
        </div>
      </div>

      <div className="card">
        <h2>Chống chỉ định ({ccd.length})</h2>
        <div className="tw">
          <table>
            <thead><tr><th>Vaccine</th><th>Lý do</th><th>Trạng thái</th></tr></thead>
            <tbody>
              {ccd.map((c) => (
                <tr key={c.id}>
                  <td>{c.vaccine}</td><td className="wrap">{c.lyDo}</td>
                  <td><span className={'tag ' + CLS_CCD[c.trangThai]}>{c.tenTrangThai}</span></td>
                </tr>
              ))}
              {ccd.length === 0 && <tr><td colSpan="3">Không có chống chỉ định nào.</td></tr>}
            </tbody>
          </table>
        </div>
      </div>
    </>
  )
}

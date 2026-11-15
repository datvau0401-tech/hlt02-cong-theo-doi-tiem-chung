import { useEffect, useState } from 'react'
import { api, thongBaoLoi } from './auth.jsx'
import { fmtGioUtc } from './format.js'

const TRANG_THAI = [['', 'Tất cả'], ['ChuaXacNhan', 'Chưa xác nhận'], ['DaXacNhan', 'Đã xác nhận'], ['DaThuHoi', 'Đã thu hồi']]
const CLS = { ChuaXacNhan: 'warn', DaXacNhan: 'bad', DaThuHoi: '' }
const TRONG = { nguoiTiemId: '', vaccineId: '', lyDo: '' }

export default function ChongChiDinhPage() {
  const [people, setPeople] = useState([])
  const [vaccines, setVaccines] = useState([])
  const [list, setList] = useState([])
  const [loc, setLoc] = useState('')
  const [form, setForm] = useState(TRONG)
  const [sua, setSua] = useState(null)     // { id, lyDo }
  const [msg, setMsg] = useState(null)

  const loadList = async (tt = loc) => setList(await api('/api/chong-chi-dinh' + (tt ? `?trangThai=${tt}` : '')))

  useEffect(() => {
    Promise.all([api('/api/nguoi-tiem'), api('/api/vaccine')])
      .then(([p, v]) => { setPeople(p); setVaccines(v) })
      .then(() => loadList())
      .catch((e) => setMsg({ type: 'err', text: thongBaoLoi(e) }))
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [])

  const set = (k) => (e) => setForm({ ...form, [k]: e.target.value })

  // Chạy một thao tác, báo kết quả rồi tải lại danh sách
  async function chay(viec, thanhCong) {
    setMsg(null)
    try {
      await viec()
      setMsg({ type: 'ok', text: thanhCong })
      await loadList()
      return true
    } catch (ex) {
      setMsg({ type: 'err', text: thongBaoLoi(ex) })
      return false
    }
  }

  async function submit(e) {
    e.preventDefault()
    const ok = await chay(() => api('/api/chong-chi-dinh', {
      method: 'POST',
      body: { nguoiTiemId: Number(form.nguoiTiemId), vaccineId: Number(form.vaccineId), lyDo: form.lyDo },
    }), 'Đã ghi nhận chống chỉ định (chờ xác nhận).')
    if (ok) setForm(TRONG)
  }

  const xacNhan = (c) => {
    if (!window.confirm(`Xác nhận ${c.nguoiTiem} KHÔNG được tiêm ${c.vaccine}?\nSau khi xác nhận, hệ thống sẽ chặn đặt lịch và ghi nhận mũi tiêm.`)) return
    chay(() => api(`/api/chong-chi-dinh/${c.id}/xac-nhan`, { method: 'POST' }), 'Đã xác nhận chống chỉ định.')
  }

  function thuHoi(c) {
    const lyDo = window.prompt(`Lý do thu hồi chống chỉ định của ${c.nguoiTiem} (${c.vaccine}):`)
    if (lyDo === null) return
    chay(() => api(`/api/chong-chi-dinh/${c.id}/thu-hoi`, { method: 'POST', body: { lyDo } }), 'Đã thu hồi chống chỉ định.')
  }

  async function luuLyDo() {
    if (await chay(() => api(`/api/chong-chi-dinh/${sua.id}`, { method: 'PUT', body: { lyDo: sua.lyDo } }), 'Đã sửa lý do.'))
      setSua(null)
  }

  return (
    <>
      <h1>Chống chỉ định</h1>

      <form className="card" onSubmit={submit}>
        <h2>Ghi nhận chống chỉ định</h2>
        <div className="row">
          <label>Người tiêm
            <select value={form.nguoiTiemId} onChange={set('nguoiTiemId')} required>
              <option value="">Chọn...</option>
              {people.map((n) => <option key={n.id} value={n.id}>{n.hoTen}{n.soDienThoai ? ` - ${n.soDienThoai}` : ''}</option>)}
            </select>
          </label>
          <label>Vaccine
            <select value={form.vaccineId} onChange={set('vaccineId')} required>
              <option value="">Chọn...</option>
              {vaccines.map((v) => <option key={v.id} value={v.id}>{v.ten}</option>)}
            </select>
          </label>
        </div>
        <div className="row">
          <label>Lý do
            <input value={form.lyDo} onChange={set('lyDo')} maxLength={300} required
                   placeholder="VD: tiền sử dị ứng nặng với thành phần vaccine" />
          </label>
        </div>
        <button type="submit">Ghi nhận</button>
        {msg && <span className={'msg ' + msg.type}>{msg.text}</span>}
      </form>

      <div className="card">
        <div className="toolbar">
          <h2>Danh sách chống chỉ định ({list.length})</h2>
          <label className="inline">Trạng thái&nbsp;
            <select value={loc} onChange={(e) => { setLoc(e.target.value); loadList(e.target.value).catch((x) => setMsg({ type: 'err', text: thongBaoLoi(x) })) }}>
              {TRANG_THAI.map(([v, t]) => <option key={v} value={v}>{t}</option>)}
            </select>
          </label>
        </div>
        <div className="tw">
          <table>
            <thead>
              <tr><th>Người tiêm</th><th>Vaccine</th><th>Lý do</th><th>Trạng thái</th><th>Người xử lý</th><th>Ghi nhận lúc</th><th></th></tr>
            </thead>
            <tbody>
              {list.map((c) => (
                <tr key={c.id}>
                  <td>{c.nguoiTiem}</td><td>{c.vaccine}</td>
                  <td className="wrap">
                    {sua?.id === c.id ? (
                      <>
                        <input value={sua.lyDo} maxLength={300} onChange={(e) => setSua({ ...sua, lyDo: e.target.value })} />
                        <button className="sm" style={{ marginLeft: 4 }} onClick={luuLyDo}>Lưu</button>
                        <button className="sec sm" style={{ marginLeft: 4 }} onClick={() => setSua(null)}>Bỏ</button>
                      </>
                    ) : c.lyDo}
                  </td>
                  <td><span className={'tag ' + CLS[c.trangThai]}>{c.tenTrangThai}</span></td>
                  <td>{c.nguoiXacNhan ?? ''}</td>
                  <td>{fmtGioUtc(c.ngayGhiNhan)}</td>
                  <td>
                    {c.trangThai === 'ChuaXacNhan' && sua?.id !== c.id && (
                      <>
                        <button className="sec sm" onClick={() => setSua({ id: c.id, lyDo: c.lyDo })}>Sửa lý do</button>
                        <button className="sm" onClick={() => xacNhan(c)}>Xác nhận</button>
                      </>
                    )}
                    {c.trangThai !== 'DaThuHoi' && <button className="danger sm" onClick={() => thuHoi(c)}>Thu hồi</button>}
                  </td>
                </tr>
              ))}
              {list.length === 0 && <tr><td colSpan="7">Chưa có chống chỉ định nào.</td></tr>}
            </tbody>
          </table>
        </div>
      </div>
    </>
  )
}

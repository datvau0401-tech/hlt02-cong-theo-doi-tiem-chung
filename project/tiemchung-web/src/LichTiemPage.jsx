import { useEffect, useState } from 'react'
import { api, thongBaoLoi } from './auth.jsx'
import { fmtNgay, homNay } from './format.js'

const TRANG_THAI = [['', 'Tất cả'], ['ChoTiem', 'Chờ tiêm'], ['DaTiem', 'Đã tiêm'], ['DaHuy', 'Đã hủy']]
const CLS = { ChoTiem: 'warn', DaTiem: 'ok', DaHuy: 'bad' }

export default function LichTiemPage() {
  const [people, setPeople] = useState([])
  const [vaccines, setVaccines] = useState([])
  const [list, setList] = useState([])
  const [loc, setLoc] = useState('ChoTiem')
  const [form, setForm] = useState({ nguoiTiemId: '', vaccineId: '', muiThu: '1', ngayHen: '' })
  const [doiNgay, setDoiNgay] = useState(null)   // { id, ngay, lyDo }
  const [msg, setMsg] = useState(null)

  const loadList = async (tt = loc) => setList(await api('/api/lich-tiem' + (tt ? `?trangThai=${tt}` : '')))

  useEffect(() => {
    Promise.all([api('/api/nguoi-tiem'), api('/api/vaccine')])
      .then(([p, v]) => { setPeople(p); setVaccines(v) })
      .then(() => loadList())
      .catch((e) => setMsg({ type: 'err', text: thongBaoLoi(e) }))
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [])

  const vc = vaccines.find((v) => String(v.id) === form.vaccineId)
  const set = (k) => (e) => setForm({ ...form, [k]: e.target.value })

  async function submit(e) {
    e.preventDefault()
    setMsg(null)
    try {
      await api('/api/lich-tiem', {
        method: 'POST',
        body: {
          nguoiTiemId: Number(form.nguoiTiemId), vaccineId: Number(form.vaccineId),
          muiThu: Number(form.muiThu), ngayHen: form.ngayHen,
        },
      })
      setMsg({ type: 'ok', text: 'Đã đặt lịch tiêm.' })
      setForm({ ...form, muiThu: '1', ngayHen: '' })
      await loadList()
    } catch (ex) {
      setMsg({ type: 'err', text: thongBaoLoi(ex) })
    }
  }

  async function luuNgay() {
    setMsg(null)
    try {
      await api(`/api/lich-tiem/${doiNgay.id}/ngay-hen`, { method: 'PUT', body: { ngayHen: doiNgay.ngay, lyDo: doiNgay.lyDo } })
      setMsg({ type: 'ok', text: 'Đã đổi ngày hẹn.' })
      setDoiNgay(null)
      await loadList()
    } catch (ex) {
      setMsg({ type: 'err', text: thongBaoLoi(ex) })
    }
  }

  async function huy(l) {
    const lyDo = window.prompt(`Lý do hủy lịch của ${l.nguoiTiem} (${l.vaccine}, mũi ${l.muiThu}):`)
    if (lyDo === null) return
    setMsg(null)
    try {
      await api(`/api/lich-tiem/${l.id}/huy`, { method: 'POST', body: { lyDo } })
      setMsg({ type: 'ok', text: 'Đã hủy lịch.' })
      await loadList()
    } catch (ex) {
      setMsg({ type: 'err', text: thongBaoLoi(ex) })
    }
  }

  const today = homNay()

  return (
    <>
      <h1>Lịch tiêm</h1>

      <form className="card" onSubmit={submit}>
        <h2>Đặt lịch tiêm</h2>
        <div className="row">
          <label>Người tiêm
            <select value={form.nguoiTiemId} onChange={set('nguoiTiemId')} required>
              <option value="">Chọn...</option>
              {people.map((p) => <option key={p.id} value={p.id}>{p.hoTen}{p.soDienThoai ? ` - ${p.soDienThoai}` : ''}</option>)}
            </select>
          </label>
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
            <input type="date" value={form.ngayHen} onChange={set('ngayHen')} min={today} required />
          </label>
        </div>
        {vc?.khoangCachNgay != null && Number(form.muiThu) > 1 &&
          <p className="muted">Mũi {form.muiThu} phải cách mũi trước ít nhất {vc.khoangCachNgay} ngày.</p>}
        <button type="submit">Đặt lịch</button>
        {msg && <span className={'msg ' + msg.type}>{msg.text}</span>}
      </form>

      <div className="card">
        <div className="toolbar">
          <h2>Danh sách lịch hẹn</h2>
          <label className="inline">Trạng thái
            <select value={loc} onChange={(e) => { setLoc(e.target.value); loadList(e.target.value).catch((x) => setMsg({ type: 'err', text: thongBaoLoi(x) })) }}>
              {TRANG_THAI.map(([v, t]) => <option key={v} value={v}>{t}</option>)}
            </select>
          </label>
        </div>
        <div className="tw">
          <table>
            <thead>
              <tr><th>Người tiêm</th><th>Vaccine</th><th>Mũi</th><th>Ngày hẹn</th><th>Trạng thái</th><th></th></tr>
            </thead>
            <tbody>
              {list.map((l) => {
                const quaHen = l.trangThai === 'ChoTiem' && l.ngayHen < today
                return (
                  <tr key={l.id}>
                    <td>{l.nguoiTiem}</td><td>{l.vaccine}</td><td>{l.muiThu}</td>
                    <td>
                      {doiNgay?.id === l.id ? (
                        <>
                          <input type="date" value={doiNgay.ngay} min={today}
                                 onChange={(e) => setDoiNgay({ ...doiNgay, ngay: e.target.value })} />
                          <input style={{ marginLeft: 6, width: 200 }} placeholder="Lý do đổi ngày (bắt buộc)" value={doiNgay.lyDo}
                                 onChange={(e) => setDoiNgay({ ...doiNgay, lyDo: e.target.value })} />
                          <button className="sm" style={{ marginLeft: 6 }} onClick={luuNgay} disabled={!doiNgay.ngay || !doiNgay.lyDo.trim()}>Lưu</button>
                          <button className="sec sm" style={{ marginLeft: 4 }} onClick={() => setDoiNgay(null)}>Bỏ</button>
                        </>
                      ) : <>{fmtNgay(l.ngayHen)} {quaHen && <span className="tag bad">Quá hẹn</span>}</>}
                    </td>
                    <td><span className={'tag ' + (CLS[l.trangThai] ?? '')}>{l.tenTrangThai}</span></td>
                    <td>
                      {l.trangThai === 'ChoTiem' && doiNgay?.id !== l.id && (
                        <>
                          <button className="sec sm" onClick={() => setDoiNgay({ id: l.id, ngay: l.ngayHen < today ? today : l.ngayHen, lyDo: '' })}>Đổi ngày</button>
                          <button className="danger sm" style={{ marginLeft: 6 }} onClick={() => huy(l)}>Hủy lịch</button>
                        </>
                      )}
                    </td>
                  </tr>
                )
              })}
              {list.length === 0 && <tr><td colSpan="6">Không có lịch hẹn nào.</td></tr>}
            </tbody>
          </table>
        </div>
      </div>
    </>
  )
}

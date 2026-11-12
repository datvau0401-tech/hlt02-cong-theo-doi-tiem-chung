import { useEffect, useState } from 'react'

export default function MuiTiemPage() {
  const [lich, setLich] = useState([])       // lịch đang chờ tiêm
  const [lots, setLots] = useState([])       // tất cả lô
  const [nv, setNv] = useState([])           // nhân viên y tế
  const [recent, setRecent] = useState([])   // các mũi tiêm gần đây
  const [form, setForm] = useState({ lichId: '', loId: '', nvId: '' })
  const [msg, setMsg] = useState(null)

  async function load() {
    const [a, b, c, d] = await Promise.all([
      fetch('/api/lich-tiem/cho-tiem'), fetch('/api/lo-vaccine'),
      fetch('/api/nhan-vien'), fetch('/api/mui-tiem'),
    ])
    setLich(await a.json()); setLots(await b.json())
    setNv(await c.json()); setRecent(await d.json())
  }

  useEffect(() => {
    load().catch(() => setMsg({ type: 'err', text: 'Không kết nối được API. Kiểm tra API đã chạy chưa.' }))
  }, [])

  const chosen = lich.find((x) => String(x.id) === form.lichId)
  const today = new Date().toLocaleDateString('sv-SE')   // dạng yyyy-MM-dd

  // Chỉ gợi ý lô đúng vaccine, còn hạn, còn số lượng (server vẫn kiểm tra lại)
  const lotOptions = chosen
    ? lots.filter((l) => l.vaccineId === chosen.vaccineId && l.hanDung >= today && l.soLuongCon > 0)
    : []

  async function submit(e) {
    e.preventDefault()
    setMsg(null)
    const res = await fetch('/api/mui-tiem', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        lichTiemId: Number(form.lichId),
        loVaccineId: Number(form.loId),
        nhanVienId: Number(form.nvId),
      }),
    })
    if (res.ok) {
      setMsg({ type: 'ok', text: 'Đã ghi nhận mũi tiêm và trừ tồn kho lô.' })
      setForm({ ...form, lichId: '', loId: '' })
      load()
    } else {
      const b = await res.json().catch(() => null)
      setMsg({ type: 'err', text: b?.message ?? b?.title ?? 'Dữ liệu không hợp lệ.' })
    }
  }

  return (
    <>
      <h1>Ghi nhận mũi tiêm</h1>

      <form className="card" onSubmit={submit}>
        <h2>Ghi nhận mới</h2>
        <div className="row">
          <label>Lịch tiêm (chờ tiêm)
            <select value={form.lichId} onChange={(e) => setForm({ ...form, lichId: e.target.value, loId: '' })} required>
              <option value="">Chọn...</option>
              {lich.map((x) => (
                <option key={x.id} value={x.id}>
                  {x.nguoiTiem} - {x.vaccine} (mũi {x.muiThu}) - hẹn {x.ngayHen}
                </option>
              ))}
            </select>
          </label>
          <label>Lô vaccine
            <select value={form.loId} onChange={(e) => setForm({ ...form, loId: e.target.value })} required disabled={!chosen}>
              <option value="">{chosen ? 'Chọn lô...' : 'Chọn lịch tiêm trước'}</option>
              {lotOptions.map((l) => (
                <option key={l.id} value={l.id}>{l.soLo} - còn {l.soLuongCon} - hạn {l.hanDung}</option>
              ))}
            </select>
          </label>
          <label>Nhân viên y tế
            <select value={form.nvId} onChange={(e) => setForm({ ...form, nvId: e.target.value })} required>
              <option value="">Chọn...</option>
              {nv.map((x) => <option key={x.id} value={x.id}>{x.hoTen}</option>)}
            </select>
          </label>
        </div>
        <button type="submit">Ghi nhận</button>
        {msg && <span className={'msg ' + msg.type}>{msg.text}</span>}
      </form>

      <div className="card">
        <h2>Mũi tiêm gần đây</h2>
        <div className="tw">
          <table>
            <thead>
              <tr><th>Người tiêm</th><th>Vaccine</th><th>Mũi</th><th>Số lô</th><th>Thời điểm</th><th>Nhân viên</th></tr>
            </thead>
            <tbody>
              {recent.map((m) => (
                <tr key={m.id}>
                  <td>{m.nguoiTiem}</td><td>{m.vaccine}</td><td>{m.muiThu}</td>
                  <td>{m.soLo}</td><td>{new Date(m.ngayGioTiem).toLocaleString('vi-VN')}</td><td>{m.nhanVien}</td>
                </tr>
              ))}
              {recent.length === 0 && <tr><td colSpan="6">Chưa có mũi tiêm nào.</td></tr>}
            </tbody>
          </table>
        </div>
      </div>
    </>
  )
}
import { useEffect, useState } from 'react'

const LOAI = [
  ['NhapThem', 'Nhập thêm'],
  ['Hong', 'Hỏng'],
  ['HaoHut', 'Hao hụt'],
  ['Huy', 'Hủy (toàn bộ số còn lại)'],
]

export default function BienDongLoPage() {
  const [lots, setLots] = useState([])
  const [nv, setNv] = useState([])
  const [history, setHistory] = useState([])
  const [form, setForm] = useState({ loId: '', loai: 'Hong', soLuong: '', nvId: '', lyDo: '' })
  const [msg, setMsg] = useState(null)

  async function load() {
    const [a, b, c] = await Promise.all([
      fetch('/api/lo-vaccine'), fetch('/api/nhan-vien'), fetch('/api/bien-dong-lo'),
    ])
    setLots(await a.json()); setNv(await b.json()); setHistory(await c.json())
  }

  useEffect(() => {
    load().catch(() => setMsg({ type: 'err', text: 'Không kết nối được API. Kiểm tra API đã chạy chưa.' }))
  }, [])

  const lo = lots.find((x) => String(x.id) === form.loId)
  const huy = form.loai === 'Huy'
  const set = (k) => (e) => setForm({ ...form, [k]: e.target.value })

  async function submit(e) {
    e.preventDefault()
    setMsg(null)
    const res = await fetch('/api/bien-dong-lo', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        loVaccineId: Number(form.loId),
        loai: form.loai,
        soLuong: huy ? (lo?.soLuongCon ?? 0) : Number(form.soLuong),
        nguoiThucHienId: Number(form.nvId),
        lyDo: form.lyDo,
      }),
    })
    if (res.ok) {
      setMsg({ type: 'ok', text: 'Đã ghi nhận biến động lô.' })
      setForm({ ...form, soLuong: '', lyDo: '' })
      load()
    } else {
      const b = await res.json().catch(() => null)
      setMsg({ type: 'err', text: b?.message ?? b?.title ?? 'Dữ liệu không hợp lệ.' })
    }
  }

  return (
    <>
      <h1>Biến động lô</h1>

      <form className="card" onSubmit={submit}>
        <h2>Ghi nhận biến động</h2>
        <div className="row">
          <label>Lô vaccine
            <select value={form.loId} onChange={set('loId')} required>
              <option value="">Chọn...</option>
              {lots.map((l) => (
                <option key={l.id} value={l.id}>{l.soLo} - {l.vaccine} (còn {l.soLuongCon}/{l.soLuongNhap})</option>
              ))}
            </select>
          </label>
          <label>Loại
            <select value={form.loai} onChange={set('loai')}>
              {LOAI.map(([v, t]) => <option key={v} value={v}>{t}</option>)}
            </select>
          </label>
          <label>Số lượng
            <input type="number" min="1" required
                   value={huy ? (lo?.soLuongCon ?? '') : form.soLuong}
                   onChange={set('soLuong')} disabled={huy} />
          </label>
          <label>Người thực hiện
            <select value={form.nvId} onChange={set('nvId')} required>
              <option value="">Chọn...</option>
              {nv.map((x) => <option key={x.id} value={x.id}>{x.hoTen}</option>)}
            </select>
          </label>
        </div>
        <div className="row">
          <label>Lý do
            <input value={form.lyDo} onChange={set('lyDo')} required placeholder="Ví dụ: vỡ lọ khi vận chuyển" />
          </label>
        </div>
        <button type="submit">Ghi nhận</button>
        {msg && <span className={'msg ' + msg.type}>{msg.text}</span>}
      </form>

      <div className="card">
        <h2>Biến động gần đây</h2>
        <div className="tw">
          <table>
            <thead>
              <tr><th>Thời điểm</th><th>Số lô</th><th>Vaccine</th><th>Loại</th><th>Số lượng</th><th>Người thực hiện</th><th>Lý do</th></tr>
            </thead>
            <tbody>
              {history.map((h) => (
                <tr key={h.id}>
                  <td>{new Date(h.thoiDiem + 'Z').toLocaleString('vi-VN')}</td>
                  <td>{h.soLo}</td><td>{h.vaccine}</td><td>{h.tenLoai}</td>
                  <td>{h.loai === 'NhapThem' ? '+' : '-'}{h.soLuong}</td>
                  <td>{h.nguoiThucHien}</td><td>{h.lyDo}</td>
                </tr>
              ))}
              {history.length === 0 && <tr><td colSpan="7">Chưa có biến động nào.</td></tr>}
            </tbody>
          </table>
        </div>
      </div>
    </>
  )
}
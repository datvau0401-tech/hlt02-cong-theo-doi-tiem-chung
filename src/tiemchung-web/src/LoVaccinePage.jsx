import { useEffect, useState } from 'react'

function tinhTrang(hanDung) {
  const days = (new Date(hanDung) - new Date()) / 864e5
  if (days < 0) return ['Hết hạn', 'bad']
  if (days < 30) return ['Sắp hết hạn', 'warn']
  return ['Còn hạn', 'ok']
}

export default function LoVaccinePage() {
  const [vaccines, setVaccines] = useState([])
  const [lots, setLots] = useState([])
  const [form, setForm] = useState({ vaccineId: '', soLo: '', hanDung: '', soLuongNhap: '' })
  const [msg, setMsg] = useState(null)

  async function load() {
    const [v, l] = await Promise.all([fetch('/api/vaccine'), fetch('/api/lo-vaccine')])
    setVaccines(await v.json())
    setLots(await l.json())
  }

  useEffect(() => {
    load().catch(() => setMsg({ type: 'err', text: 'Không kết nối được API. Kiểm tra API đã chạy chưa.' }))
  }, [])

  const set = (k) => (e) => setForm({ ...form, [k]: e.target.value })

  async function submit(e) {
    e.preventDefault()
    setMsg(null)
    const res = await fetch('/api/lo-vaccine', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        vaccineId: Number(form.vaccineId),
        soLo: form.soLo,
        hanDung: form.hanDung,
        soLuongNhap: Number(form.soLuongNhap),
      }),
    })
    if (res.ok) {
      setMsg({ type: 'ok', text: 'Đã nhập lô mới.' })
      setForm({ ...form, soLo: '', hanDung: '', soLuongNhap: '' })
      load()
    } else {
      const b = await res.json().catch(() => null)
      setMsg({ type: 'err', text: b?.message ?? b?.title ?? 'Dữ liệu không hợp lệ.' })
    }
  }

  return (
    <>
      <h1>Quản lý lô vaccine</h1>

      <form className="card" onSubmit={submit}>
        <h2>Nhập lô mới</h2>
        <div className="row">
          <label>Vaccine
            <select value={form.vaccineId} onChange={set('vaccineId')} required>
              <option value="">Chọn...</option>
              {vaccines.map((v) => <option key={v.id} value={v.id}>{v.ten}</option>)}
            </select>
          </label>
          <label>Số lô
            <input value={form.soLo} onChange={set('soLo')} required />
          </label>
          <label>Hạn dùng
            <input type="date" value={form.hanDung} onChange={set('hanDung')} required />
          </label>
          <label>Số lượng nhập
            <input type="number" min="1" value={form.soLuongNhap} onChange={set('soLuongNhap')} required />
          </label>
        </div>
        <button type="submit">Lưu lô</button>
        {msg && <span className={'msg ' + msg.type}>{msg.text}</span>}
      </form>

      <div className="card">
        <h2>Danh sách lô</h2>
        <div className="tw">
          <table>
            <thead>
              <tr><th>Số lô</th><th>Vaccine</th><th>Hạn dùng</th><th>Nhập</th><th>Còn</th><th>Tình trạng</th></tr>
            </thead>
            <tbody>
              {lots.map((l) => {
                const [text, cls] = tinhTrang(l.hanDung)
                return (
                  <tr key={l.id}>
                    <td>{l.soLo}</td><td>{l.vaccine}</td><td>{l.hanDung}</td>
                    <td>{l.soLuongNhap}</td><td>{l.soLuongCon}</td>
                    <td><span className={'tag ' + cls}>{text}</span></td>
                  </tr>
                )
              })}
            </tbody>
          </table>
        </div>
      </div>
    </>
  )
}
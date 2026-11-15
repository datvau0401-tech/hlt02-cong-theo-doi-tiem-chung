import { useEffect, useState } from 'react'
import { api, thongBaoLoi } from './auth.jsx'
import { fmtNgay, homNay } from './format.js'

const GIOI_TINH = [['Nam', 'Nam'], ['Nu', 'Nữ'], ['Khac', 'Khác']]
const TEN_GIOI = Object.fromEntries(GIOI_TINH)
const TRONG = { hoTen: '', ngaySinh: '', gioiTinh: '', soDienThoai: '', email: '', cccd: '' }

function tuoi(ngaySinh) {
  const s = new Date(ngaySinh), n = new Date()
  let t = n.getFullYear() - s.getFullYear()
  if (n.getMonth() < s.getMonth() || (n.getMonth() === s.getMonth() && n.getDate() < s.getDate())) t--
  return t
}

export default function NguoiTiemPage() {
  const [list, setList] = useState([])
  const [form, setForm] = useState(TRONG)
  const [editId, setEditId] = useState(null)
  const [q, setQ] = useState('')
  const [msg, setMsg] = useState(null)

  const load = async () => setList(await api('/api/nguoi-tiem'))

  useEffect(() => {
    load().catch((e) => setMsg({ type: 'err', text: thongBaoLoi(e) }))
  }, [])

  const set = (k) => (e) => setForm({ ...form, [k]: e.target.value })

  async function submit(e) {
    e.preventDefault()
    setMsg(null)
    const body = {
      hoTen: form.hoTen,
      ngaySinh: form.ngaySinh,
      gioiTinh: form.gioiTinh || null,
      soDienThoai: form.soDienThoai || null,
      email: form.email || null,
      cccd: form.cccd || null,
    }
    try {
      if (editId) await api(`/api/nguoi-tiem/${editId}`, { method: 'PUT', body })
      else await api('/api/nguoi-tiem', { method: 'POST', body })
      setMsg({ type: 'ok', text: editId ? 'Đã cập nhật người tiêm.' : 'Đã thêm người tiêm.' })
      huySua()
      await load()
    } catch (ex) {
      setMsg({ type: 'err', text: thongBaoLoi(ex) })
    }
  }

  function sua(n) {
    setMsg(null)
    setEditId(n.id)
    setForm({
      hoTen: n.hoTen, ngaySinh: n.ngaySinh, gioiTinh: n.gioiTinh ?? '',
      soDienThoai: n.soDienThoai ?? '', email: n.email ?? '', cccd: n.cccd ?? '',
    })
    window.scrollTo({ top: 0, behavior: 'smooth' })
  }

  function huySua() {
    setEditId(null)
    setForm(TRONG)
  }

  const k = q.trim().toLowerCase()
  const hienThi = k
    ? list.filter((n) => [n.hoTen, n.soDienThoai, n.email, n.cccd].some((v) => v?.toLowerCase().includes(k)))
    : list

  return (
    <>
      <h1>Người tiêm</h1>

      <form className="card" onSubmit={submit}>
        <h2>{editId ? 'Sửa thông tin người tiêm' : 'Thêm người tiêm'}</h2>
        <div className="row">
          <label>Họ và tên
            <input value={form.hoTen} onChange={set('hoTen')} maxLength={100} required />
          </label>
          <label>Ngày sinh
            <input type="date" value={form.ngaySinh} onChange={set('ngaySinh')} max={homNay()} required />
          </label>
          <label>Giới tính
            <select value={form.gioiTinh} onChange={set('gioiTinh')}>
              <option value="">Chưa rõ</option>
              {GIOI_TINH.map(([v, t]) => <option key={v} value={v}>{t}</option>)}
            </select>
          </label>
        </div>
        <div className="row">
          <label>Số điện thoại
            <input type="tel" value={form.soDienThoai} onChange={set('soDienThoai')} placeholder="VD: 0912345678" />
          </label>
          <label>Email
            <input type="email" value={form.email} onChange={set('email')} maxLength={100} />
          </label>
          <label>Số CCCD
            <input value={form.cccd} onChange={set('cccd')} inputMode="numeric" maxLength={12} pattern="[0-9]{12}" title="Đúng 12 chữ số" placeholder="12 chữ số" />
          </label>
        </div>
        <button type="submit">{editId ? 'Cập nhật' : 'Thêm'}</button>
        {editId && <button type="button" className="sec" style={{ marginLeft: 8 }} onClick={huySua}>Hủy sửa</button>}
        {msg && <span className={'msg ' + msg.type}>{msg.text}</span>}
      </form>

      <div className="card">
        <div className="toolbar">
          <h2>Danh sách người tiêm ({hienThi.length})</h2>
          <input className="search" placeholder="Tìm theo tên, SĐT, email..." value={q} onChange={(e) => setQ(e.target.value)} />
        </div>
        <div className="tw">
          <table>
            <thead>
              <tr><th>Họ và tên</th><th>Ngày sinh</th><th>Tuổi</th><th>Giới tính</th><th>SĐT</th><th>Email</th><th>CCCD</th><th></th></tr>
            </thead>
            <tbody>
              {hienThi.map((n) => (
                <tr key={n.id}>
                  <td>{n.hoTen}</td><td>{fmtNgay(n.ngaySinh)}</td><td>{tuoi(n.ngaySinh)}</td>
                  <td>{TEN_GIOI[n.gioiTinh] ?? ''}</td><td>{n.soDienThoai}</td><td>{n.email}</td><td>{n.cccd}</td>
                  <td><button className="sec sm" onClick={() => sua(n)}>Sửa</button></td>
                </tr>
              ))}
              {hienThi.length === 0 && <tr><td colSpan="8">Không có người tiêm nào.</td></tr>}
            </tbody>
          </table>
        </div>
      </div>
    </>
  )
}

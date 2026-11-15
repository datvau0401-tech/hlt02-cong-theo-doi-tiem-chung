import { useEffect, useState } from 'react'
import { api, thongBaoLoi } from './auth.jsx'

const VAI_TRO = [['NhanVienYTe', 'Bác sĩ / nhân viên y tế'], ['QuanTri', 'Quản trị viên']]
const TRONG = { hoTen: '', vaiTro: 'NhanVienYTe', tenDangNhap: '', matKhau: '' }

export default function TaiKhoanPage() {
  const [list, setList] = useState([])
  const [form, setForm] = useState(TRONG)
  const [msg, setMsg] = useState(null)

  const load = async () => setList(await api('/api/tai-khoan'))

  useEffect(() => {
    load().catch((e) => setMsg({ type: 'err', text: thongBaoLoi(e) }))
  }, [])

  const set = (k) => (e) => setForm({ ...form, [k]: e.target.value })

  async function submit(e) {
    e.preventDefault()
    setMsg(null)
    try {
      await api('/api/tai-khoan', { method: 'POST', body: form })
      setMsg({ type: 'ok', text: 'Đã tạo tài khoản.' })
      setForm(TRONG)
      await load()
    } catch (ex) {
      setMsg({ type: 'err', text: thongBaoLoi(ex) })
    }
  }

  // Đặt mật khẩu mới; tài khoản chưa có tên đăng nhập thì hỏi thêm tên đăng nhập
  async function datMatKhau(t) {
    let tenDangNhap = null
    if (!t.tenDangNhap) {
      tenDangNhap = window.prompt(`Tên đăng nhập cho ${t.hoTen} (chữ không dấu, số, . _ -):`)
      if (tenDangNhap === null) return
    }
    const matKhau = window.prompt(`Mật khẩu mới cho ${t.hoTen} (từ 8 ký tự, có cả chữ và số):`)
    if (matKhau === null) return
    setMsg(null)
    try {
      await api(`/api/tai-khoan/${t.id}/dang-nhap`, { method: 'PUT', body: { tenDangNhap, matKhau } })
      setMsg({ type: 'ok', text: 'Đã đặt mật khẩu.' })
      await load()
    } catch (ex) {
      setMsg({ type: 'err', text: thongBaoLoi(ex) })
    }
  }

  return (
    <>
      <h1>Tài khoản</h1>

      <form className="card" onSubmit={submit}>
        <h2>Cấp tài khoản cho bác sĩ / quản trị viên</h2>
        <div className="row">
          <label>Họ và tên
            <input value={form.hoTen} onChange={set('hoTen')} maxLength={100} required placeholder="VD: BS. Nguyễn A" />
          </label>
          <label>Vai trò
            <select value={form.vaiTro} onChange={set('vaiTro')}>
              {VAI_TRO.map(([v, t]) => <option key={v} value={v}>{t}</option>)}
            </select>
          </label>
        </div>
        <div className="row">
          <label>Tên đăng nhập
            <input value={form.tenDangNhap} onChange={set('tenDangNhap')} minLength={3} maxLength={50} required autoComplete="off" />
          </label>
          <label>Mật khẩu (từ 8 ký tự, có cả chữ và số)
            <input type="password" value={form.matKhau} onChange={set('matKhau')} minLength={8} required autoComplete="new-password" />
          </label>
        </div>
        <button type="submit">Tạo tài khoản</button>
        {msg && <span className={'msg ' + msg.type}>{msg.text}</span>}
      </form>

      <div className="card">
        <h2>Danh sách tài khoản ({list.length})</h2>
        <p className="muted">Người dân tự đăng ký ở trang đăng nhập. Bạn chỉ cần cấp tài khoản cho bác sĩ và quản trị viên.</p>
        <div className="tw">
          <table>
            <thead>
              <tr><th>Họ và tên</th><th>Vai trò</th><th>Tên đăng nhập</th><th>Mật khẩu</th><th></th></tr>
            </thead>
            <tbody>
              {list.map((t) => (
                <tr key={t.id}>
                  <td>{t.hoTen}</td><td>{t.tenVaiTro}</td><td>{t.tenDangNhap ?? ''}</td>
                  <td><span className={'tag ' + (t.coMatKhau ? 'ok' : 'warn')}>{t.coMatKhau ? 'Đã đặt' : 'Chưa đặt'}</span></td>
                  <td><button className="sec sm" onClick={() => datMatKhau(t)}>{t.tenDangNhap ? 'Đặt lại mật khẩu' : 'Cấp đăng nhập'}</button></td>
                </tr>
              ))}
              {list.length === 0 && <tr><td colSpan="5">Chưa có tài khoản nào.</td></tr>}
            </tbody>
          </table>
        </div>
      </div>
    </>
  )
}

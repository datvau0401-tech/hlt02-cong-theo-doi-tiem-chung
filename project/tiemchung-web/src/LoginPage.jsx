import { useState } from 'react'
import { useAuth, thongBaoLoi } from './auth.jsx'

export default function LoginPage({ onRegister }) {
  const { login } = useAuth()
  const [form, setForm] = useState({ tenDangNhap: '', matKhau: '' })
  const [hien, setHien] = useState(false)
  const [busy, setBusy] = useState(false)
  const [err, setErr] = useState('')

  const set = (k) => (e) => setForm({ ...form, [k]: e.target.value })

  async function submit(e) {
    e.preventDefault()
    setErr('')
    setBusy(true)
    try {
      await login(form.tenDangNhap.trim(), form.matKhau)
    } catch (ex) {
      setErr(thongBaoLoi(ex))
      setBusy(false)
    }
  }

  return (
    <div className="login-wrap">
      <form className="card login-card" onSubmit={submit}>
          <img src="/logo.png" alt="Logo" className="login-logo" />
        <h1>Tiêm chủng cộng đồng</h1>
        <p className="muted">Đăng nhập để tiếp tục</p>

        {err && <div className="alert err">{err}</div>}

        <label>Tên đăng nhập
          <input value={form.tenDangNhap} onChange={set('tenDangNhap')}
                 autoComplete="username" autoFocus required />
        </label>
        <label>Mật khẩu
          <span className="pw">
            <input type={hien ? 'text' : 'password'} value={form.matKhau} onChange={set('matKhau')}
                   autoComplete="current-password" required />
            <button type="button" className="link" onClick={() => setHien(!hien)}>{hien ? 'Ẩn' : 'Hiện'}</button>
          </span>
        </label>

        <button type="submit" disabled={busy}>{busy ? 'Đang đăng nhập...' : 'Đăng nhập'}</button>
        <p className="muted">Người dân chưa có tài khoản? <button type="button" className="link" onClick={onRegister}>Đăng ký</button></p>
      </form>
    </div>
  )
}

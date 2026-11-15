import { useState } from 'react'
import { useAuth, thongBaoLoi } from './auth.jsx'
import { homNay } from './format.js'

const TRONG = { hoTen: '', ngaySinh: '', gioiTinh: '', soDienThoai: '', email: '', cccd: '', tenDangNhap: '', matKhau: '', nhapLai: '' }

export default function RegisterPage({ onLogin }) {
  const { register } = useAuth()
  const [form, setForm] = useState(TRONG)
  const [hien, setHien] = useState(false)
  const [busy, setBusy] = useState(false)
  const [err, setErr] = useState('')

  const set = (k) => (e) => setForm({ ...form, [k]: e.target.value })

  async function submit(e) {
    e.preventDefault()
    setErr('')
    if (form.matKhau !== form.nhapLai) return setErr('Hai lần nhập mật khẩu không giống nhau.')
    setBusy(true)
    try {
      await register({
        hoTen: form.hoTen,
        ngaySinh: form.ngaySinh,
        gioiTinh: form.gioiTinh || null,
        soDienThoai: form.soDienThoai || null,
        email: form.email || null,
        cccd: form.cccd,
        tenDangNhap: form.tenDangNhap.trim(),
        matKhau: form.matKhau,
      })
    } catch (ex) {
      setErr(thongBaoLoi(ex))
      setBusy(false)
    }
  }

  const R = <i className="req"> (*)</i>

  return (
    <div className="reg-page">
      <header className="reg-top">
        <b>CỔNG THEO DÕI TIÊM CHỦNG CỘNG ĐỒNG</b>
        <button type="button" className="link" onClick={onLogin}>Đăng nhập</button>
      </header>
      <form className="reg-form" onSubmit={submit}>
        <div className="reg-title">
          <h1>Đăng ký tài khoản người dân</h1>
          <span className="muted">Trang chủ / Đăng ký</span>
        </div>
        <p className="muted" style={{ margin: '0 0 16px' }}>Dành cho người dân muốn đặt lịch và xem lịch sử tiêm.</p>

        {err && <div className="alert err">{err}</div>}

        <div className="fgrid">
          <label>Họ và tên{R}
            <input value={form.hoTen} onChange={set('hoTen')} maxLength={100} placeholder="Nhập họ và tên" required autoFocus />
          </label>
          <label>Ngày sinh{R}
            <input type="date" value={form.ngaySinh} onChange={set('ngaySinh')} max={homNay()} required />
          </label>
          <label>Giới tính
            <select value={form.gioiTinh} onChange={set('gioiTinh')}>
              <option value="">Giới tính</option>
              <option value="Nam">Nam</option>
              <option value="Nu">Nữ</option>
              <option value="Khac">Khác</option>
            </select>
          </label>
          <label>Số CMND/CCCD{R}
            <input value={form.cccd} onChange={set('cccd')} inputMode="numeric" maxLength={12} pattern="[0-9]{12}" title="Đúng 12 chữ số" placeholder="Nhập số căn cước (12 số)" required />
          </label>
          <label>Số điện thoại
            <input type="tel" value={form.soDienThoai} onChange={set('soDienThoai')} placeholder="Nhập số điện thoại" />
          </label>
          <label>Email
            <input type="email" value={form.email} onChange={set('email')} maxLength={100} placeholder="Nhập email" />
          </label>
          <label>Tên đăng nhập{R}
            <input value={form.tenDangNhap} onChange={set('tenDangNhap')} autoComplete="username" minLength={3} maxLength={50}
                   placeholder="Chữ không dấu, số, . _ -" required />
          </label>
          <label>Mật khẩu (từ 8 ký tự, có cả chữ và số){R}
            <span className="pw">
              <input type={hien ? 'text' : 'password'} value={form.matKhau} onChange={set('matKhau')}
                     autoComplete="new-password" minLength={8} placeholder="Nhập mật khẩu" required />
              <button type="button" className="link" onClick={() => setHien(!hien)}>{hien ? 'Ẩn' : 'Hiện'}</button>
            </span>
          </label>
          <label>Nhập lại mật khẩu{R}
            <input type={hien ? 'text' : 'password'} value={form.nhapLai} onChange={set('nhapLai')}
                   autoComplete="new-password" placeholder="Nhập lại mật khẩu" required />
          </label>
        </div>

        <div className="reg-actions">
          <button type="button" className="sec" onClick={() => setForm(TRONG)}>Nhập lại</button>
          <button type="submit" disabled={busy}>{busy ? 'Đang tạo tài khoản...' : 'Đăng ký'}</button>
        </div>
        <p className="muted" style={{ textAlign: 'center' }}>Đã có tài khoản? <button type="button" className="link" onClick={onLogin}>Đăng nhập</button></p>
      </form>
    </div>
  )
}

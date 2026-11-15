import { useState } from 'react'
import './App.css'
import { useAuth } from './auth.jsx'
import LoginPage from './LoginPage.jsx'
import RegisterPage from './RegisterPage.jsx'
import DashboardPage from './DashboardPage.jsx'
import VaccinePage from './VaccinePage.jsx'
import LoVaccinePage from './LoVaccinePage.jsx'
import BienDongLoPage from './BienDongLoPage.jsx'
import NguoiTiemPage from './NguoiTiemPage.jsx'
import ChongChiDinhPage from './ChongChiDinhPage.jsx'
import LichTiemPage from './LichTiemPage.jsx'
import MuiTiemPage from './MuiTiemPage.jsx'
import TruyVetPage from './TruyVetPage.jsx'
import NhatKyPage from './NhatKyPage.jsx'
import TaiKhoanPage from './TaiKhoanPage.jsx'
import CuaToiPage from './CuaToiPage.jsx'

// [tên menu, trang]
const MENU_NHAN_VIEN = [
  ['Dashboard', DashboardPage],
  ['Vaccine', VaccinePage],
  ['Lô vaccine', LoVaccinePage],
  ['Biến động lô', BienDongLoPage],
  ['Người tiêm', NguoiTiemPage],
  ['Chống chỉ định', ChongChiDinhPage],
  ['Lịch tiêm', LichTiemPage],
  ['Ghi nhận mũi tiêm', MuiTiemPage],
  ['Truy vết lô', TruyVetPage],
  ['Lịch sử hiệu chỉnh', NhatKyPage],
]
const MENU_QUAN_TRI = [...MENU_NHAN_VIEN, ['Tài khoản', TaiKhoanPage]]
const MENU_NGUOI_DAN = [['Trang của tôi', CuaToiPage]]

const MENU_THEO_VAI_TRO = { QuanTri: MENU_QUAN_TRI, NhanVienYTe: MENU_NHAN_VIEN, NguoiDan: MENU_NGUOI_DAN }
const TEN_VAI_TRO = { QuanTri: 'Quản trị viên', NhanVienYTe: 'Bác sĩ / nhân viên y tế', NguoiDan: 'Người dân' }

export default function App() {
  const { user, loading, logout } = useAuth()
  const [page, setPage] = useState('')
  const [dangKy, setDangKy] = useState(false)

  if (loading) return <p className="muted" style={{ padding: 24 }}>Đang tải...</p>
  if (!user) {
    return dangKy
      ? <RegisterPage onLogin={() => setDangKy(false)} />
      : <LoginPage onRegister={() => setDangKy(true)} />
  }

  const menu = MENU_THEO_VAI_TRO[user.vaiTro] ?? MENU_NGUOI_DAN
  const dang = menu.find(([ten]) => ten === page) ?? menu[0]
  const Page = dang[1]

  function dangXuat() {
    setPage('')
    setDangKy(false)
    logout()
  }

  return (
    <div className="app">
      <aside className="side">
        <div className="brand"><span>Tiêm chủng<br />cộng đồng</span></div>
        <nav>
          {menu.map(([ten]) => (
            <div key={ten} className={ten === dang[0] ? 'item on' : 'item'} onClick={() => setPage(ten)}>{ten}</div>
          ))}
        </nav>
        <div className="who">
          <div className="avatar">{(user.hoTen ?? '?').trim().charAt(0).toUpperCase()}</div>
          <div className="who-info">
            <b>{user.hoTen}</b>
            <span>{TEN_VAI_TRO[user.vaiTro] ?? user.vaiTro}</span>
          </div>
          <button type="button" className="sec sm" onClick={dangXuat}>Đăng xuất</button>
        </div>
      </aside>
      <main className="pane"><Page /></main>
    </div>
  )
}

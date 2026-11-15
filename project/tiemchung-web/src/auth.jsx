import { createContext, useContext, useEffect, useState } from 'react'

// Mọi lệnh fetch tới /api/... mà bị trả 401 (hết phiên đăng nhập) thì báo cho AuthProvider
// để quay về trang đăng nhập. Làm ở đây một lần nên các trang cũ không cần sửa.
let khiHetPhien = () => {}
const rawFetch = window.fetch.bind(window)
window.fetch = async (input, init) => {
  const res = await rawFetch(input, init)
  const url = typeof input === 'string' ? input : (input?.url ?? '')
  if (res.status === 401 && url.startsWith('/api/') && !url.startsWith('/api/auth/')) khiHetPhien()
  return res
}

// Gọi API trả JSON; lỗi thì ném Error có sẵn câu thông báo từ server
export async function api(url, { method = 'GET', body } = {}) {
  const res = await fetch(url, {
    method,
    headers: body !== undefined ? { 'Content-Type': 'application/json' } : undefined,
    body: body !== undefined ? JSON.stringify(body) : undefined,
  })
  if (res.ok) return res.status === 204 ? null : res.json()
  const b = await res.json().catch(() => null)
  throw new Error(b?.message ?? b?.title ?? 'Dữ liệu không hợp lệ.')
}

// Không có phản hồi từ server (API chưa chạy) thì fetch ném TypeError
export const thongBaoLoi = (e) =>
  e instanceof TypeError ? 'Không kết nối được API. Kiểm tra API đã chạy chưa.' : e.message

const AuthContext = createContext(null)
// eslint-disable-next-line react-refresh/only-export-components
export const useAuth = () => useContext(AuthContext)

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    khiHetPhien = () => setUser(null)
    api('/api/auth/me')
      .then(setUser)
      .catch(() => setUser(null))
      .finally(() => setLoading(false))
  }, [])

  async function login(tenDangNhap, matKhau) {
    setUser(await api('/api/auth/login', { method: 'POST', body: { tenDangNhap, matKhau } }))
  }

  // Người dân tự đăng ký: server tạo tài khoản và đăng nhập luôn
  async function register(body) {
    setUser(await api('/api/auth/dang-ky', { method: 'POST', body }))
  }

  async function logout() {
    await fetch('/api/auth/logout', { method: 'POST' }).catch(() => {})
    setUser(null)
  }

  return <AuthContext.Provider value={{ user, loading, login, register, logout }}>{children}</AuthContext.Provider>
}

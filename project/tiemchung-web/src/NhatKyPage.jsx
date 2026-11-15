import { useEffect, useState } from 'react'
import { api, thongBaoLoi } from './auth.jsx'
import { fmtGioUtc } from './format.js'

const BANG = [['', 'Tất cả'], ['NguoiTiem', 'Người tiêm'], ['LichTiem', 'Lịch tiêm'], ['Vaccine', 'Vaccine'], ['ChongChiDinh', 'Chống chỉ định'], ['NguoiDung', 'Tài khoản']]
const HANH_DONG = { THEM: ['Thêm', 'ok'], SUA: ['Sửa', 'warn'], HUY: ['Hủy', 'bad'] }
const NHAN = {
  hoTen: 'Họ tên', ngaySinh: 'Ngày sinh', gioiTinh: 'Giới tính', soDienThoai: 'SĐT', email: 'Email', cccd: 'CCCD',
  nguoiTiem: 'Người tiêm', vaccine: 'Vaccine', muiThu: 'Mũi', ngayHen: 'Ngày hẹn', trangThai: 'Trạng thái',
  ten: 'Tên', nhaSanXuat: 'Nhà sản xuất', soMui: 'Số mũi', khoangCachNgay: 'Cách nhau (ngày)', lyDo: 'Lý do', vaiTro: 'Vai trò', tenDangNhap: 'Tên đăng nhập',
}

function parse(s) {
  try { return s ? JSON.parse(s) : null } catch { return null }
}
const hien = (v) => (v === null || v === undefined || v === '' ? '(trống)' : String(v))

// Chỉ hiện các trường thay đổi (sửa/hủy) hoặc toàn bộ giá trị (thêm)
function chiTiet(n) {
  const cu = parse(n.giaTriCu), moi = parse(n.giaTriMoi)
  if (!moi) return null
  if (!cu) return Object.entries(moi).map(([k, v]) => `${NHAN[k] ?? k}: ${hien(v)}`)
  return Object.keys(moi)
    .filter((k) => cu[k] !== moi[k])
    .map((k) => `${NHAN[k] ?? k}: ${hien(cu[k])} → ${hien(moi[k])}`)
}

export default function NhatKyPage() {
  const [list, setList] = useState([])
  const [bang, setBang] = useState('')
  const [q, setQ] = useState('')
  const [msg, setMsg] = useState('')

  useEffect(() => {
    const t = setTimeout(() => {
      const p = new URLSearchParams()
      if (bang) p.set('tenBang', bang)
      if (q.trim()) p.set('tuKhoa', q.trim())
      api('/api/nhat-ky?' + p).then((d) => { setList(d); setMsg('') }).catch((e) => setMsg(thongBaoLoi(e)))
    }, 250)
    return () => clearTimeout(t)
  }, [bang, q])

  return (
    <>
      <h1>Lịch sử hiệu chỉnh</h1>

      <div className="card">
        <div className="toolbar">
          <h2>Nhật ký thao tác ({list.length})</h2>
          <div className="filters">
            <label className="inline">Bảng
              <select value={bang} onChange={(e) => setBang(e.target.value)}>
                {BANG.map(([v, t]) => <option key={v} value={v}>{t}</option>)}
              </select>
            </label>
            <input className="search" placeholder="Tìm theo mã bản ghi, nội dung, lý do..." value={q} onChange={(e) => setQ(e.target.value)} />
          </div>
        </div>
        {msg && <span className="msg err">{msg}</span>}
        <div className="tw">
          <table>
            <thead>
              <tr><th>Thời điểm</th><th>Người thực hiện</th><th>Hành động</th><th>Bảng</th><th>Mã</th><th>Chi tiết</th><th>Lý do</th></tr>
            </thead>
            <tbody>
              {list.map((n) => {
                const [ten, cls] = HANH_DONG[n.hanhDong] ?? [n.hanhDong, '']
                const ct = chiTiet(n)
                return (
                  <tr key={n.id}>
                    <td>{fmtGioUtc(n.thoiDiem)}</td><td>{n.nguoiSua}</td>
                    <td><span className={'tag ' + cls}>{ten}</span></td>
                    <td>{BANG.find(([v]) => v === n.tenBang)?.[1] ?? n.tenBang}</td><td>{n.maBanGhi}</td>
                    <td className="wrap">{ct ? ct.map((s) => <div key={s}>{s}</div>) : ''}</td>
                    <td className="wrap">{n.lyDo}</td>
                  </tr>
                )
              })}
              {list.length === 0 && <tr><td colSpan="7">Chưa có thay đổi nào được ghi lại.</td></tr>}
            </tbody>
          </table>
        </div>
      </div>
    </>
  )
}

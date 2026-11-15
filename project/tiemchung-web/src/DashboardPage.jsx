import { useEffect, useState } from 'react'
import { api, thongBaoLoi } from './auth.jsx'
import { fmtNgay } from './format.js'

export default function DashboardPage() {
  const [d, setD] = useState(null)
  const [err, setErr] = useState('')

  useEffect(() => {
    api('/api/dashboard').then(setD).catch((e) => setErr(thongBaoLoi(e)))
  }, [])

  if (err) return <><h1>Dashboard</h1><span className="msg err">{err}</span></>
  if (!d) return <><h1>Dashboard</h1><p className="muted">Đang tải...</p></>

  const cards = [
    ['Lô đang quản lý', d.soLo],
    ['Tồn kho (lô còn hạn)', d.tongTon],
    ['Lô sắp hết hạn', d.loSapHetHan, d.loSapHetHan > 0 ? 'warn' : ''],
    ['Lô hết hạn còn tồn', d.loHetHanConTon, d.loHetHanConTon > 0 ? 'bad' : ''],
    ['Người tiêm', d.soNguoiTiem],
    ['Lịch chờ tiêm', d.lichChoTiem],
    ['Lịch quá hẹn', d.lichQuaHen, d.lichQuaHen > 0 ? 'warn' : ''],
    ['Mũi tiêm hôm nay', d.muiTiemHomNay],
  ]

  return (
    <>
      <h1>Dashboard</h1>

      <div className="stats">
        {cards.map(([ten, so, cls]) => (
          <div key={ten} className={'stat ' + (cls || '')}><span>{ten}</span><b>{so}</b></div>
        ))}
      </div>

      <div className="card">
        <h2>Lô cần chú ý</h2>
        <div className="tw">
          <table>
            <thead>
              <tr><th>Số lô</th><th>Vaccine</th><th>Hạn dùng</th><th>Còn</th><th>Ghi chú</th></tr>
            </thead>
            <tbody>
              {d.loCanChuY.map((l) => (
                <tr key={l.id}>
                  <td>{l.soLo}</td><td>{l.vaccine}</td><td>{fmtNgay(l.hanDung)}</td><td>{l.soLuongCon}</td>
                  <td><span className={'tag ' + l.muc}>{l.ghiChu}</span></td>
                </tr>
              ))}
              {d.loCanChuY.length === 0 && <tr><td colSpan="5">Không có lô nào cần chú ý.</td></tr>}
            </tbody>
          </table>
        </div>
      </div>
    </>
  )
}

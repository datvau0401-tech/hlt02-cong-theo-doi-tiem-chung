import { useEffect, useState } from 'react'
import { api, thongBaoLoi } from './auth.jsx'
import { fmtNgay, fmtGio } from './format.js'

export default function TruyVetPage() {
  const [mode, setMode] = useState('lo')            // 'lo' = theo lô, 'nguoi' = theo người tiêm
  const [lots, setLots] = useState([])
  const [people, setPeople] = useState([])
  const [chon, setChon] = useState('')
  const [kq, setKq] = useState(null)
  const [msg, setMsg] = useState(null)

  useEffect(() => {
    Promise.all([api('/api/lo-vaccine'), api('/api/nguoi-tiem')])
      .then(([l, p]) => { setLots(l); setPeople(p) })
      .catch((e) => setMsg(thongBaoLoi(e)))
  }, [])

  function doiMode(m) {
    setMode(m); setChon(''); setKq(null); setMsg(null)
  }

  async function tra(id) {
    setChon(id); setKq(null); setMsg(null)
    if (!id) return
    try {
      setKq(await api(mode === 'lo' ? `/api/truy-vet/lo/${id}` : `/api/truy-vet/nguoi-tiem/${id}`))
    } catch (ex) {
      setMsg(thongBaoLoi(ex))
    }
  }

  const dong = kq?.daTiem ?? []

  return (
    <>
      <h1>Truy vết lô</h1>

      <div className="card">
        <div className="tabs">
          <button className={mode === 'lo' ? '' : 'sec'} onClick={() => doiMode('lo')}>Theo lô vaccine</button>
          <button className={mode === 'nguoi' ? '' : 'sec'} onClick={() => doiMode('nguoi')}>Theo người tiêm</button>
        </div>
        <div className="row">
          {mode === 'lo' ? (
            <label>Lô vaccine
              <select value={chon} onChange={(e) => tra(e.target.value)}>
                <option value="">Chọn lô cần truy vết...</option>
                {lots.map((l) => <option key={l.id} value={l.id}>{l.soLo} - {l.vaccine}</option>)}
              </select>
            </label>
          ) : (
            <label>Người tiêm
              <select value={chon} onChange={(e) => tra(e.target.value)}>
                <option value="">Chọn người tiêm...</option>
                {people.map((p) => <option key={p.id} value={p.id}>{p.hoTen}{p.soDienThoai ? ` - ${p.soDienThoai}` : ''}</option>)}
              </select>
            </label>
          )}
        </div>
        {msg && <span className="msg err">{msg}</span>}
      </div>

      {kq && mode === 'lo' && (
        <div className="stats">
          <div className="stat"><span>Vaccine</span><b className="sm">{kq.vaccine}</b></div>
          <div className="stat"><span>Nơi sản xuất</span><b className="sm">{kq.nhaSanXuat} ({kq.quocGia})</b></div>
          <div className="stat"><span>Hạn dùng</span><b className="sm">{fmtNgay(kq.hanDung)}</b></div>
          <div className="stat"><span>Đã nhập</span><b>{kq.soLuongNhap}</b></div>
          <div className="stat"><span>Đã tiêm</span><b>{dong.length}</b></div>
          <div className="stat"><span>Còn lại</span><b>{kq.soLuongCon}</b></div>
        </div>
      )}

      {kq && (
        <div className="card">
          <h2>
            {mode === 'lo' ? `Những người đã tiêm lô ${kq.soLo} (${dong.length})` : `Các mũi đã tiêm của ${kq.nguoiTiem.hoTen} (${dong.length})`}
          </h2>
          <div className="tw">
            <table>
              <thead>
                <tr>
                  {mode === 'lo' ? <><th>Người tiêm</th><th>SĐT</th></> : <><th>Số lô</th><th>Nơi sản xuất</th><th>Hạn dùng lô</th></>}
                  <th>Vaccine</th><th>Mũi</th><th>Thời điểm</th><th>Nhân viên</th>
                </tr>
              </thead>
              <tbody>
                {dong.map((d) => (
                  <tr key={d.muiTiemId}>
                    {mode === 'lo' ? <><td>{d.hoTen}</td><td>{d.soDienThoai}</td></> : <><td>{d.soLo}</td><td>{d.nhaSanXuat} ({d.quocGia})</td><td>{fmtNgay(d.hanDungLo)}</td></>}
                    <td>{d.vaccine}</td><td>{d.muiThu}</td><td>{fmtGio(d.ngayGioTiem)}</td><td>{d.nhanVien}</td>
                  </tr>
                ))}
                {dong.length === 0 && <tr><td colSpan="7">{mode === 'lo' ? 'Chưa có ai tiêm từ lô này.' : 'Người này chưa tiêm mũi nào.'}</td></tr>}
              </tbody>
            </table>
          </div>
        </div>
      )}
    </>
  )
}

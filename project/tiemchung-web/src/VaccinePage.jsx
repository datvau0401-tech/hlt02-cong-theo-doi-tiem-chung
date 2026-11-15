import { useEffect, useState } from 'react'
import { api, thongBaoLoi, useAuth } from './auth.jsx'

const TRONG = { ten: '', nhaSanXuat: '', soMui: '1', khoangCachNgay: '' }

export default function VaccinePage() {
  const { user } = useAuth()
  const quanTri = user?.vaiTro === 'QuanTri'      // chỉ quản trị viên được thêm/sửa danh mục
  const [list, setList] = useState([])
  const [form, setForm] = useState(TRONG)
  const [editId, setEditId] = useState(null)
  const [q, setQ] = useState('')
  const [msg, setMsg] = useState(null)

  const load = async () => setList(await api('/api/vaccine'))

  useEffect(() => {
    load().catch((e) => setMsg({ type: 'err', text: thongBaoLoi(e) }))
  }, [])

  const set = (k) => (e) => setForm({ ...form, [k]: e.target.value })
  const nhieuMui = Number(form.soMui) > 1

  async function submit(e) {
    e.preventDefault()
    setMsg(null)
    const body = {
      ten: form.ten,
      nhaSanXuat: form.nhaSanXuat || null,
      soMui: Number(form.soMui),
      khoangCachNgay: nhieuMui && form.khoangCachNgay !== '' ? Number(form.khoangCachNgay) : null,
    }
    try {
      if (editId) await api(`/api/vaccine/${editId}`, { method: 'PUT', body })
      else await api('/api/vaccine', { method: 'POST', body })
      setMsg({ type: 'ok', text: editId ? 'Đã cập nhật vaccine.' : 'Đã thêm vaccine.' })
      huySua()
      await load()
    } catch (ex) {
      setMsg({ type: 'err', text: thongBaoLoi(ex) })
    }
  }

  function sua(v) {
    setMsg(null)
    setEditId(v.id)
    setForm({
      ten: v.ten, nhaSanXuat: v.nhaSanXuat ?? '', soMui: String(v.soMui),
      khoangCachNgay: String(v.khoangCachNgay ?? ''),
    })
    window.scrollTo({ top: 0, behavior: 'smooth' })
  }

  function huySua() {
    setEditId(null)
    setForm(TRONG)
  }

  const k = q.trim().toLowerCase()
  const hienThi = k ? list.filter((v) => [v.ten, v.nhaSanXuat].some((x) => x?.toLowerCase().includes(k))) : list

  return (
    <>
      <h1>Vaccine</h1>

      {quanTri ? (
        <form className="card" onSubmit={submit}>
          <h2>{editId ? 'Sửa vaccine' : 'Thêm vaccine'}</h2>
          <div className="row">
            <label>Tên vaccine
              <input value={form.ten} onChange={set('ten')} maxLength={100} required placeholder="VD: Vaccine cúm" />
            </label>
            <label>Nhà sản xuất
              <input value={form.nhaSanXuat} onChange={set('nhaSanXuat')} maxLength={100} />
            </label>
            <label>Số mũi
              <input type="number" min="1" max="10" value={form.soMui} onChange={set('soMui')} required />
            </label>
            <label>Cách nhau tối thiểu (ngày)
              <input type="number" min="0" value={nhieuMui ? form.khoangCachNgay : ''} onChange={set('khoangCachNgay')}
                     disabled={!nhieuMui} required={nhieuMui} placeholder={nhieuMui ? 'VD: 28' : 'Chỉ 1 mũi'} />
            </label>
          </div>
          <button type="submit">{editId ? 'Cập nhật' : 'Thêm'}</button>
          {editId && <button type="button" className="sec" style={{ marginLeft: 8 }} onClick={huySua}>Hủy sửa</button>}
          {msg && <span className={'msg ' + msg.type}>{msg.text}</span>}
        </form>
      ) : (
        <p className="muted">Chỉ quản trị viên được thêm hoặc sửa danh mục vaccine.</p>
      )}

      <div className="card">
        <div className="toolbar">
          <h2>Danh mục vaccine ({hienThi.length})</h2>
          <input className="search" placeholder="Tìm theo tên, nhà sản xuất..." value={q} onChange={(e) => setQ(e.target.value)} />
        </div>
        {!quanTri && msg && <span className={'msg ' + msg.type}>{msg.text}</span>}
        <div className="tw">
          <table>
            <thead>
              <tr><th>Tên vaccine</th><th>Nhà sản xuất</th><th>Số mũi</th><th>Khoảng cách giữa 2 mũi</th>{quanTri && <th></th>}</tr>
            </thead>
            <tbody>
              {hienThi.map((v) => (
                <tr key={v.id}>
                  <td>{v.ten}</td><td>{v.nhaSanXuat}</td><td>{v.soMui}</td>
                  <td>{v.khoangCachNgay != null ? `${v.khoangCachNgay} ngày` : ''}</td>
                  {quanTri && <td><button className="sec sm" onClick={() => sua(v)}>Sửa</button></td>}
                </tr>
              ))}
              {hienThi.length === 0 && <tr><td colSpan="5">Không có vaccine nào.</td></tr>}
            </tbody>
          </table>
        </div>
      </div>
    </>
  )
}

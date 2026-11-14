import { useState } from 'react'
import './App.css'
import LoVaccinePage from './LoVaccinePage.jsx'
import MuiTiemPage from './MuiTiemPage.jsx'
import BienDongLoPage from './BienDongLoPage.jsx'

const MENU = ['Dashboard', 'Lô vaccine', 'Biến động lô', 'Người tiêm', 'Lịch tiêm',
              'Ghi nhận mũi tiêm', 'Truy vết lô', 'Lịch sử hiệu chỉnh']

// Menu nào đã làm xong thì mới bấm được
const PAGES = {
  'Lô vaccine': <LoVaccinePage />,
  'Biến động lô': <BienDongLoPage />,
  'Ghi nhận mũi tiêm': <MuiTiemPage />,
}

export default function App() {
  const [page, setPage] = useState('Lô vaccine')

  return (
    <div className="app">
      <aside className="side">
        <div className="brand">Tiêm chủng cộng đồng</div>
        {MENU.map((m) => (
          <div key={m}
               className={m === page ? 'item on' : PAGES[m] ? 'item' : 'item off'}
               style={PAGES[m] ? { cursor: 'pointer' } : undefined}
               onClick={() => PAGES[m] && setPage(m)}>
            {m}
          </div>
        ))}
      </aside>
      <main className="pane">{PAGES[page]}</main>
    </div>
  )
}
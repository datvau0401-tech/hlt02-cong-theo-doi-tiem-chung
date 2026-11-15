// yyyy-MM-dd -> dd/MM/yyyy
export const fmtNgay = (s) => {
  if (!s) return ''
  const [y, m, d] = s.slice(0, 10).split('-')
  return `${d}/${m}/${y}`
}

// Giờ do server lưu theo UTC (SYSUTCDATETIME) nhưng JSON không có chữ Z nên phải thêm vào
export const fmtGioUtc = (s) => (s ? new Date(s.endsWith('Z') ? s : s + 'Z').toLocaleString('vi-VN') : '')

// Giờ theo máy chủ (DateTime.Now), không đổi múi giờ
export const fmtGio = (s) => (s ? new Date(s).toLocaleString('vi-VN') : '')

// Ngày hôm nay dạng yyyy-MM-dd theo giờ máy
export const homNay = () => new Date().toLocaleDateString('sv-SE')

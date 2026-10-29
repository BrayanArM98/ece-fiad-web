/**
 * Formatos de fecha, hora y cifras en español de México.
 * La API guarda las fechas sin zona horaria: se interpretan y se envían como hora local.
 */

const fechaLarga = new Intl.DateTimeFormat('es-MX', { weekday: 'long', day: 'numeric', month: 'long' })
const fechaCorta = new Intl.DateTimeFormat('es-MX', { day: '2-digit', month: 'short', year: 'numeric' })
const hora = new Intl.DateTimeFormat('es-MX', { hour: '2-digit', minute: '2-digit', hour12: false })
const mes = new Intl.DateTimeFormat('es-MX', { month: 'long' })

/**
 * Convierte lo que manda la API en fecha local. Acepta "2026-10-29" (solo fecha, que JavaScript
 * interpretaría en UTC) y "2026-10-29T10:30:00" (fecha y hora sin zona, que se toma como local).
 */
export function aFecha(valor: string | Date): Date {
  if (valor instanceof Date) return valor
  return new Date(/^\d{4}-\d{2}-\d{2}$/.test(valor) ? `${valor}T00:00:00` : valor)
}

export const formatearFechaLarga = (valor: string | Date) => fechaLarga.format(aFecha(valor))
export const formatearFechaCorta = (valor: string | Date) => fechaCorta.format(aFecha(valor))
export const formatearHora = (valor: string | Date) => hora.format(aFecha(valor))
export const nombreMes = (valor: string | Date) => mes.format(aFecha(valor))

const dosDigitos = (n: number) => String(n).padStart(2, '0')

/** "2026-10-29": fecha local para los filtros de la API. */
export function aFechaApi(fecha: Date) {
  return `${fecha.getFullYear()}-${dosDigitos(fecha.getMonth() + 1)}-${dosDigitos(fecha.getDate())}`
}

/** "2026-10-29T10:30:00": fecha y hora local sin zona, como la guarda la API. */
export function aFechaHoraApi(fecha: Date) {
  return `${aFechaApi(fecha)}T${dosDigitos(fecha.getHours())}:${dosDigitos(fecha.getMinutes())}:00`
}

export function formatearCifra(valor: number, formato: 'numero' | 'porcentaje' = 'numero') {
  return formato === 'porcentaje'
    ? `${valor.toLocaleString('es-MX', { maximumFractionDigits: 1 })}%`
    : valor.toLocaleString('es-MX', { maximumFractionDigits: 0 })
}

export function mayuscula(texto: string) {
  return texto.charAt(0).toUpperCase() + texto.slice(1)
}

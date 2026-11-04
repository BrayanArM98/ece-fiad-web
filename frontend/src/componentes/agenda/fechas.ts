import { addDays, format, isValid, parse, startOfWeek } from 'date-fns'
import { es } from 'date-fns/locale'
import { aFecha, mayuscula } from '@/lib/formato'

/**
 * Fechas de la agenda. Las semanas empiezan en lunes y las citas duran 30 minutos
 * (la API no guarda duración: cada cita ocupa un espacio de media hora).
 */

export const DURACION_CITA_MIN = 30
/** Horario que siempre se muestra en la vista de día; se amplía si hay citas fuera de él. */
export const HORA_INICIO = 8
export const HORA_FIN = 20

export type Vista = 'dia' | 'semana'

export const inicioSemana = (fecha: Date) => startOfWeek(fecha, { weekStartsOn: 1 })
export const diasDeLaSemana = (fecha: Date) => Array.from({ length: 7 }, (_, i) => addDays(inicioSemana(fecha), i))

/** Lee "2026-11-04" de la URL; si no es válida usa hoy. */
export function leerFecha(texto: string | null): Date {
  if (!texto) return hoy()
  const fecha = parse(texto, 'yyyy-MM-dd', new Date())
  return isValid(fecha) ? fecha : hoy()
}

export function hoy() {
  const ahora = new Date()
  return new Date(ahora.getFullYear(), ahora.getMonth(), ahora.getDate())
}

/** "Miércoles 4 de noviembre de 2026" o "2 – 8 de noviembre de 2026". */
export function tituloRango(fecha: Date, vista: Vista) {
  if (vista === 'dia') return mayuscula(format(fecha, "EEEE d 'de' MMMM 'de' yyyy", { locale: es }))

  const [lunes, domingo] = [inicioSemana(fecha), addDays(inicioSemana(fecha), 6)]
  if (lunes.getMonth() === domingo.getMonth()) {
    return `${format(lunes, 'd')} – ${format(domingo, "d 'de' MMMM 'de' yyyy", { locale: es })}`
  }
  if (lunes.getFullYear() === domingo.getFullYear()) {
    return `${format(lunes, "d 'de' MMMM", { locale: es })} – ${format(domingo, "d 'de' MMMM 'de' yyyy", { locale: es })}`
  }
  return `${format(lunes, "d 'de' MMMM 'de' yyyy", { locale: es })} – ${format(domingo, "d 'de' MMMM 'de' yyyy", { locale: es })}`
}

/** Minutos desde la medianoche de la fecha y hora de una cita. */
export function minutosDelDia(valor: string | Date) {
  const fecha = aFecha(valor)
  return fecha.getHours() * 60 + fecha.getMinutes()
}

/** "09:30" a partir de minutos desde la medianoche. */
export function textoHora(minutos: number) {
  return `${String(Math.floor(minutos / 60)).padStart(2, '0')}:${String(minutos % 60).padStart(2, '0')}`
}

/** Horarios que se ofrecen al agendar, cada media hora dentro del horario de la agenda. */
export const HORARIOS = Array.from(
  { length: ((HORA_FIN - HORA_INICIO) * 60) / DURACION_CITA_MIN },
  (_, i) => textoHora(HORA_INICIO * 60 + i * DURACION_CITA_MIN),
)

/** Une una fecha y un horario "09:30" en una sola fecha local. */
export function unirFechaHora(fecha: Date, horario: string) {
  const [horas, minutos] = horario.split(':').map(Number)
  return new Date(fecha.getFullYear(), fecha.getMonth(), fecha.getDate(), horas, minutos)
}

import type { Genero, GrupoSanguineo, Rol, TipoDocumento } from '@/lib/tipos'
import { aFecha } from '@/lib/formato'

/** Textos de los catálogos de la API (los enums llegan con su nombre en código). */

export const TEXTO_DOCUMENTO: Record<TipoDocumento, string> = {
  Cedula: 'Cédula',
  Pasaporte: 'Pasaporte',
  Otro: 'Otro',
}

export const TEXTO_GENERO: Record<Genero, string> = {
  Masculino: 'Masculino',
  Femenino: 'Femenino',
  Otro: 'Otro',
  PrefieroNoDecir: 'Prefiero no decir',
}

export const TEXTO_GRUPO: Record<GrupoSanguineo, string> = {
  APositivo: 'A+',
  ANegativo: 'A−',
  BPositivo: 'B+',
  BNegativo: 'B−',
  ABPositivo: 'AB+',
  ABNegativo: 'AB−',
  OPositivo: 'O+',
  ONegativo: 'O−',
}

export const ROLES: Rol[] = ['Administrador', 'Recepcionista', 'Medico', 'Paciente']

/** Años cumplidos a hoy. */
export function edad(fechaNacimiento: string) {
  const nacimiento = aFecha(fechaNacimiento)
  const hoy = new Date()
  let anios = hoy.getFullYear() - nacimiento.getFullYear()
  const mes = hoy.getMonth() - nacimiento.getMonth()
  if (mes < 0 || (mes === 0 && hoy.getDate() < nacimiento.getDate())) anios--
  return anios
}

export const textoEdad = (fechaNacimiento: string) => {
  const anios = edad(fechaNacimiento)
  return anios === 1 ? '1 año' : `${anios} años`
}

/** "EXP-00012": número de expediente a partir del id del paciente. */
export const numeroExpediente = (idPaciente: number) => `EXP-${String(idPaciente).padStart(5, '0')}`

/** Sin acentos ni mayúsculas, para búsquedas. */
export const normalizar = (texto: string) =>
  texto
    .normalize('NFD')
    .replace(/\p{Diacritic}/gu, '')
    .toLowerCase()

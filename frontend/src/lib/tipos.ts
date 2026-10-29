/**
 * Tipos de la API ECE-FIAD. Coinciden con los DTOs del backend
 * (los enums llegan como texto, por ejemplo "Pendiente" o "Medico").
 */

export type Rol = 'Administrador' | 'Recepcionista' | 'Medico' | 'Paciente'

export interface Usuario {
  id: number
  email: string
  nombreCompleto: string
  rol: Rol
  idDoctor: number | null
  idPaciente: number | null
  activo: boolean
  ultimoAcceso: string | null
  fechaDeCreacion: string
}

export interface Sesion {
  token: string
  expiraUtc: string
  usuario: Usuario
}

export type EstadoCita = 'Pendiente' | 'Confirmada' | 'Cancelada' | 'Completada' | 'NoAsistio'

export interface Cita {
  id: number
  idPaciente: number
  idDoctor: number
  nombrePaciente: string
  nombreDoctor: string
  fechaHora: string
  motivo: string
  notas: string
  estado: EstadoCita
  estadoTexto: string
  activo: boolean
}

// ===== Reportes =====

export interface Periodo {
  /** Fecha sin zona, por ejemplo "2026-10-01T00:00:00" */
  desde: string
  hasta: string
  dias: number
}

export interface Conteo {
  etiqueta: string
  cantidad: number
  porcentaje: number
}

export interface Indicador {
  clave: string
  titulo: string
  valor: number
  valorAnterior: number | null
  variacion: number | null
  formato: 'numero' | 'porcentaje'
}

export interface ResumenGeneral {
  periodo: Periodo
  periodoAnterior: Periodo
  indicadores: Indicador[]
}

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

export interface PuntoSerieCitas {
  fecha: string
  total: number
  completadas: number
  noAsistio: number
  canceladas: number
  porAtender: number
}

export interface ReporteCitas {
  periodo: Periodo
  total: number
  agrupacion: 'dia' | 'semana'
  porEstado: Conteo[]
  serie: PuntoSerieCitas[]
  porDoctor: Conteo[]
  porEspecialidad: Conteo[]
}

export interface ProductividadDoctor {
  idDoctor: number
  nombreDoctor: string
  especialidad: string
  citasTotales: number
  completadas: number
  noAsistio: number
  canceladas: number
  porAtender: number
  evoluciones: number
  tasaAsistencia: number | null
}

export interface ReporteProductividad {
  periodo: Periodo
  doctores: ProductividadDoctor[]
}

export interface ReportePacientes {
  periodo: Periodo
  pacientesActivos: number
  nuevosEnPeriodo: number
  porGenero: Conteo[]
  porRangoEdad: Conteo[]
  porGrupoSanguineo: Conteo[]
}

export interface ReporteDiagnosticos {
  periodo: Periodo
  totalEvoluciones: number
  diagnosticos: Conteo[]
}

// ===== Catálogos =====

export interface Doctor {
  id: number
  nombres: string
  apellidos: string
  nombreCompleto: string
  telefono: string
  email: string
  horarioAtencion: string
  idEspecialidad: number
  nombreEspecialidad: string
  activo: boolean
  cantidadCitas: number
}

export type TipoDocumento = 'Cedula' | 'Pasaporte' | 'Otro'
export type Genero = 'Masculino' | 'Femenino' | 'Otro' | 'PrefieroNoDecir'
export type GrupoSanguineo =
  | 'APositivo'
  | 'ANegativo'
  | 'BPositivo'
  | 'BNegativo'
  | 'ABPositivo'
  | 'ABNegativo'
  | 'OPositivo'
  | 'ONegativo'

export interface Paciente {
  id: number
  nombres: string
  apellidos: string
  numeroDocumento: string
  tipoDocumento: TipoDocumento
  telefono: string | null
  email: string
  fechaNacimiento: string
  genero: Genero
  grupoSanguineo: GrupoSanguineo
  direccion: string | null
  activo: boolean
}

export interface Especialidad {
  id: number
  nombre: string
  descripcion: string | null
  activo: boolean
  cantidadDoctores: number
}

// ===== Expediente =====

export interface HistoriaClinica {
  id: number
  idPaciente: number
  nombrePaciente: string
  fechaApertura: string
  alergias: string
  antecedentesFamiliares: string
  antecedentesPersonales: string
  activo: boolean
  fechaDeCreacion: string
  fechaDeModificacion: string | null
}

export interface Evolucion {
  id: number
  idHistoriaClinica: number
  idDoctor: number
  nombrePaciente: string
  nombreDoctor: string
  nombreEspecialidad: string
  fecha: string
  diagnostico: string
  tratamiento: string
  notas: string
  activo: boolean
  fechaDeCreacion: string
  fechaDeModificacion: string | null
}

export interface HistorialPaciente {
  tieneHistoria: boolean
  historia: HistoriaClinica | null
  evoluciones: Evolucion[]
  totalEvoluciones: number
}

// ===== Citas: datos que se envían =====

export interface DatosCita {
  idPaciente: number
  idDoctor: number
  /** Fecha y hora local sin zona, por ejemplo "2026-11-04T09:30:00" */
  fechaHora: string
  motivo: string
  notas: string
  estado: EstadoCita
}

/** Resultado que se registra con PATCH /api/Citas/{id}/estado. */
export type ResultadoCita = Extract<EstadoCita, 'Confirmada' | 'Completada' | 'NoAsistio'>

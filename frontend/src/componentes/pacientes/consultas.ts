import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { pedir } from '@/lib/api'
import type {
  Cita,
  Evolucion,
  Genero,
  GrupoSanguineo,
  HistoriaClinica,
  HistorialPaciente,
  Paciente,
  TipoDocumento,
} from '@/lib/tipos'

/** Consultas y acciones de pacientes, historia clínica y evoluciones. */

export interface DatosNuevoPaciente {
  nombres: string
  apellidos: string
  tipoDocumento: TipoDocumento
  numeroDocumento: string
  fechaNacimiento: string
  genero: Genero
  grupoSanguineo: GrupoSanguineo
  telefono: string
  email: string
  direccion: string
}

export interface DatosEdicionPaciente {
  nombres: string
  apellidos: string
  telefono: string
  email: string
  direccion: string
}

export interface DatosHistoria {
  alergias: string
  antecedentesPersonales: string
  antecedentesFamiliares: string
}

export interface DatosEvolucion {
  idDoctor: number
  diagnostico: string
  tratamiento: string
  notas: string
}

export function usePacientes() {
  return useQuery({
    queryKey: ['pacientes', 'todos'],
    queryFn: () => pedir<Paciente[]>('/api/Pacientes'),
    select: (pacientes) =>
      [...pacientes].sort((a, b) =>
        `${a.apellidos} ${a.nombres}`.localeCompare(`${b.apellidos} ${b.nombres}`, 'es'),
      ),
  })
}

export function usePaciente(id: number) {
  return useQuery({
    queryKey: ['pacientes', id],
    queryFn: () => pedir<Paciente>(`/api/Pacientes/${id}`),
    enabled: id > 0,
  })
}

/** Historia clínica y evoluciones del paciente (solo administración y médicos). */
export function useHistorial(idPaciente: number, habilitada: boolean) {
  return useQuery({
    queryKey: ['historial', idPaciente],
    queryFn: () => pedir<HistorialPaciente>(`/api/historial/paciente/${idPaciente}`),
    enabled: habilitada && idPaciente > 0,
  })
}

export function useCitasPaciente(idPaciente: number) {
  return useQuery({
    queryKey: ['citas', { idPaciente }],
    queryFn: () => pedir<Cita[]>('/api/Citas', { parametros: { idPaciente } }),
    select: (citas) => [...citas].sort((a, b) => b.fechaHora.localeCompare(a.fechaHora)),
    enabled: idPaciente > 0,
  })
}

function useInvalidar(...claves: unknown[][]) {
  const cliente = useQueryClient()
  return () => Promise.all(claves.map((queryKey) => cliente.invalidateQueries({ queryKey })))
}

export function useRegistrarPaciente() {
  const invalidar = useInvalidar(['pacientes'], ['reportes'])
  return useMutation({
    mutationFn: (datos: DatosNuevoPaciente) => pedir<Paciente>('/api/Pacientes', { metodo: 'POST', cuerpo: datos }),
    onSuccess: invalidar,
  })
}

export function useEditarPaciente() {
  const invalidar = useInvalidar(['pacientes'])
  return useMutation({
    mutationFn: ({ id, datos }: { id: number; datos: DatosEdicionPaciente }) =>
      pedir<Paciente>(`/api/Pacientes/${id}`, { metodo: 'PUT', cuerpo: { id, ...datos } }),
    onSuccess: invalidar,
  })
}

export function useDarDeBajaPaciente() {
  const invalidar = useInvalidar(['pacientes'], ['reportes'])
  return useMutation({
    mutationFn: (id: number) => pedir<void>(`/api/Pacientes/${id}`, { metodo: 'DELETE' }),
    onSuccess: invalidar,
  })
}

export function useGuardarHistoria(idPaciente: number) {
  const invalidar = useInvalidar(['historial', idPaciente])
  return useMutation({
    mutationFn: ({ historia, datos }: { historia: HistoriaClinica | null; datos: DatosHistoria }) =>
      historia
        ? pedir<HistoriaClinica>(`/api/Historias/${historia.id}`, {
            metodo: 'PUT',
            cuerpo: {
              id: historia.id,
              idPaciente,
              fechaApertura: historia.fechaApertura,
              activo: true,
              ...datos,
            },
          })
        : pedir<HistoriaClinica>('/api/Historias', {
            metodo: 'POST',
            cuerpo: { idPaciente, activo: true, ...datos },
          }),
    onSuccess: invalidar,
  })
}

export function useGuardarEvolucion(idPaciente: number) {
  const invalidar = useInvalidar(['historial', idPaciente], ['reportes'])
  return useMutation({
    mutationFn: ({
      idHistoria,
      evolucion,
      datos,
      fecha,
    }: {
      idHistoria: number
      evolucion: Evolucion | null
      datos: DatosEvolucion
      fecha: string
    }) =>
      evolucion
        ? pedir<Evolucion>(`/api/Evoluciones/${evolucion.id}`, {
            metodo: 'PUT',
            cuerpo: { id: evolucion.id, idHistoriaClinica: idHistoria, fecha: evolucion.fecha, activo: true, ...datos },
          })
        : pedir<Evolucion>('/api/Evoluciones', {
            metodo: 'POST',
            cuerpo: { idHistoriaClinica: idHistoria, fecha, activo: true, ...datos },
          }),
    onSuccess: invalidar,
  })
}

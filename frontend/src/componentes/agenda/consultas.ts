import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { pedir } from '@/lib/api'
import type { Cita, DatosCita, Doctor, Paciente, ResultadoCita } from '@/lib/tipos'

/**
 * Consultas y acciones de la agenda contra la API.
 * Toda acción sobre una cita invalida las listas de citas y los reportes,
 * porque los indicadores de Inicio dependen de ellas.
 */

export interface FiltroCitas {
  /** "2026-11-04" */
  desde: string
  hasta: string
  idDoctor?: number
}

export function useCitas(filtro: FiltroCitas, habilitada = true) {
  return useQuery({
    queryKey: ['citas', filtro],
    queryFn: () => pedir<Cita[]>('/api/Citas', { parametros: { ...filtro } }),
    select: (citas) => [...citas].sort((a, b) => a.fechaHora.localeCompare(b.fechaHora)),
    enabled: habilitada,
  })
}

export function useDoctores() {
  return useQuery({
    queryKey: ['doctores'],
    queryFn: () => pedir<Doctor[]>('/api/Doctores'),
    select: (doctores) =>
      doctores.filter((d) => d.activo).sort((a, b) => a.nombreCompleto.localeCompare(b.nombreCompleto, 'es')),
    staleTime: 5 * 60_000,
  })
}

export function usePacientesActivos(habilitada = true) {
  return useQuery({
    queryKey: ['pacientes', 'activos'],
    queryFn: () => pedir<Paciente[]>('/api/Pacientes/activos'),
    select: (pacientes) =>
      [...pacientes].sort((a, b) =>
        `${a.apellidos} ${a.nombres}`.localeCompare(`${b.apellidos} ${b.nombres}`, 'es'),
      ),
    staleTime: 60_000,
    enabled: habilitada,
  })
}

function useAlTerminar() {
  const cliente = useQueryClient()
  return () =>
    Promise.all([
      cliente.invalidateQueries({ queryKey: ['citas'] }),
      cliente.invalidateQueries({ queryKey: ['reportes'] }),
    ])
}

export function useAgendarCita() {
  const alTerminar = useAlTerminar()
  return useMutation({
    mutationFn: (datos: DatosCita) => pedir<Cita>('/api/Citas', { metodo: 'POST', cuerpo: datos }),
    onSuccess: alTerminar,
  })
}

export function useEditarCita() {
  const alTerminar = useAlTerminar()
  return useMutation({
    mutationFn: ({ id, datos }: { id: number; datos: DatosCita }) =>
      pedir<Cita>(`/api/Citas/${id}`, { metodo: 'PUT', cuerpo: { id, ...datos } }),
    onSuccess: alTerminar,
  })
}

export function useCancelarCita() {
  const alTerminar = useAlTerminar()
  return useMutation({
    mutationFn: (id: number) => pedir<Cita>(`/api/Citas/${id}/cancelar`, { metodo: 'PATCH' }),
    onSuccess: alTerminar,
  })
}

export function useRegistrarResultado() {
  const alTerminar = useAlTerminar()
  return useMutation({
    mutationFn: ({ id, estado }: { id: number; estado: ResultadoCita }) =>
      pedir<Cita>(`/api/Citas/${id}/estado`, { metodo: 'PATCH', cuerpo: { estado } }),
    onSuccess: alTerminar,
  })
}

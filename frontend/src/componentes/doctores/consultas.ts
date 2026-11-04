import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { pedir } from '@/lib/api'
import type { Doctor, Especialidad } from '@/lib/tipos'

/** Consultas y acciones de doctores y especialidades (las acciones son solo del administrador). */

export interface DatosDoctor {
  nombres: string
  apellidos: string
  telefono: string
  email: string
  horarioAtencion: string
  idEspecialidad: number
}

export interface DatosEspecialidad {
  nombre: string
  descripcion: string
}

/** Todos los doctores, activos o no (la agenda usa useDoctores, que solo trae los activos). */
export function useListaDoctores() {
  return useQuery({
    queryKey: ['doctores'],
    queryFn: () => pedir<Doctor[]>('/api/Doctores'),
    select: (doctores) => [...doctores].sort((a, b) => a.nombreCompleto.localeCompare(b.nombreCompleto, 'es')),
  })
}

export function useEspecialidades() {
  return useQuery({
    queryKey: ['especialidades'],
    queryFn: () => pedir<Especialidad[]>('/api/Especialidades'),
    select: (lista) => [...lista].sort((a, b) => a.nombre.localeCompare(b.nombre, 'es')),
    staleTime: 5 * 60_000,
  })
}

function useInvalidarCatalogos() {
  const cliente = useQueryClient()
  return () =>
    Promise.all([
      cliente.invalidateQueries({ queryKey: ['doctores'] }),
      cliente.invalidateQueries({ queryKey: ['especialidades'] }),
    ])
}

export function useGuardarDoctor() {
  const invalidar = useInvalidarCatalogos()
  return useMutation({
    mutationFn: ({ id, datos }: { id?: number; datos: DatosDoctor }) =>
      id
        ? pedir<Doctor>(`/api/Doctores/${id}`, { metodo: 'PUT', cuerpo: { id, ...datos } })
        : pedir<Doctor>('/api/Doctores', { metodo: 'POST', cuerpo: datos }),
    onSuccess: invalidar,
  })
}

export function useEliminarDoctor() {
  const invalidar = useInvalidarCatalogos()
  return useMutation({
    mutationFn: (id: number) => pedir<void>(`/api/Doctores/${id}`, { metodo: 'DELETE' }),
    onSuccess: invalidar,
  })
}

export function useGuardarEspecialidad() {
  const invalidar = useInvalidarCatalogos()
  return useMutation({
    mutationFn: ({ id, datos }: { id?: number; datos: DatosEspecialidad }) =>
      id
        ? pedir<Especialidad>(`/api/Especialidades/${id}`, { metodo: 'PUT', cuerpo: { id, ...datos } })
        : pedir<Especialidad>('/api/Especialidades', { metodo: 'POST', cuerpo: datos }),
    onSuccess: invalidar,
  })
}

export function useEliminarEspecialidad() {
  const invalidar = useInvalidarCatalogos()
  return useMutation({
    mutationFn: (id: number) => pedir<void>(`/api/Especialidades/${id}`, { metodo: 'DELETE' }),
    onSuccess: invalidar,
  })
}

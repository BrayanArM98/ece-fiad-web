import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { pedir } from '@/lib/api'
import type { Rol, Usuario } from '@/lib/tipos'

/** Cuentas de acceso (solo el administrador). */

export interface DatosNuevoUsuario {
  email: string
  contrasena: string
  nombreCompleto: string
  rol: Rol
  idDoctor: number | null
  idPaciente: number | null
}

export interface DatosEdicionUsuario {
  nombreCompleto: string
  rol: Rol
  idDoctor: number | null
  idPaciente: number | null
  activo: boolean
  nuevaContrasena: string | null
}

export function useUsuarios() {
  return useQuery({
    queryKey: ['usuarios'],
    queryFn: () => pedir<Usuario[]>('/api/Usuarios'),
    select: (usuarios) => [...usuarios].sort((a, b) => a.nombreCompleto.localeCompare(b.nombreCompleto, 'es')),
  })
}

function useInvalidar() {
  const cliente = useQueryClient()
  return () => cliente.invalidateQueries({ queryKey: ['usuarios'] })
}

export function useCrearUsuario() {
  const invalidar = useInvalidar()
  return useMutation({
    mutationFn: (datos: DatosNuevoUsuario) => pedir<Usuario>('/api/Usuarios', { metodo: 'POST', cuerpo: datos }),
    onSuccess: invalidar,
  })
}

export function useEditarUsuario() {
  const invalidar = useInvalidar()
  return useMutation({
    mutationFn: ({ id, datos }: { id: number; datos: DatosEdicionUsuario }) =>
      pedir<Usuario>(`/api/Usuarios/${id}`, { metodo: 'PUT', cuerpo: { id, ...datos } }),
    onSuccess: invalidar,
  })
}

export function useEliminarUsuario() {
  const invalidar = useInvalidar()
  return useMutation({
    mutationFn: (id: number) => pedir<void>(`/api/Usuarios/${id}`, { metodo: 'DELETE' }),
    onSuccess: invalidar,
  })
}

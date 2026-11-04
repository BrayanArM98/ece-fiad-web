import { Eye, EyeOff } from 'lucide-react'
import { useId, useState, type FormEvent } from 'react'
import { toast } from 'sonner'
import { SelectorPaciente } from '@/componentes/agenda/SelectorPaciente'
import { useListaDoctores } from '@/componentes/doctores/consultas'
import { AvisoError, Campo, conError, PieFormulario } from '@/componentes/Formulario'
import { usePacientes } from '@/componentes/pacientes/consultas'
import { Button } from '@/components/ui/button'
import { Checkbox } from '@/components/ui/checkbox'
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle } from '@/components/ui/dialog'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'
import { ROLES } from '@/lib/catalogos'
import type { Rol, Usuario } from '@/lib/tipos'
import { TEXTO_ROL } from '@/sesion/permisos'
import { useSesion } from '@/sesion/SesionContext'
import { useCrearUsuario, useEditarUsuario } from './consultas'

const CORREO = /^[^\s@]+@[^\s@]+\.[^\s@]+$/

const DESCRIPCION_ROL: Record<Rol, string> = {
  Administrador: 'Acceso completo, incluidos usuarios y catálogos',
  Recepcionista: 'Agenda, pacientes y reporte de citas',
  Medico: 'Su agenda, expedientes y sus reportes',
  Paciente: 'Solo la app móvil, con sus propias citas',
}

/** Mínimo 8 caracteres con al menos una letra y un número (ReglasUsuario.ContrasenaSegura). */
function errorContrasena(contrasena: string) {
  if (contrasena.length < 8) return 'Debe tener al menos 8 caracteres.'
  if (!/[A-Za-z]/.test(contrasena)) return 'Debe incluir al menos una letra.'
  if (!/[0-9]/.test(contrasena)) return 'Debe incluir al menos un número.'
  return undefined
}

/**
 * Crear o editar una cuenta (specs/frontend-pantallas.md, "Usuarios").
 * Un médico se vincula con su doctor y un paciente con su expediente; los demás roles no llevan vínculo.
 */
export function FormularioUsuario({
  abierto,
  alCerrar,
  usuario,
  usuarios,
}: {
  abierto: boolean
  alCerrar: () => void
  usuario: Usuario | null
  /** Para marcar los doctores y pacientes que ya tienen cuenta. */
  usuarios: Usuario[]
}) {
  return (
    <Dialog open={abierto} onOpenChange={(siguiente) => !siguiente && alCerrar()}>
      <DialogContent className="max-h-[calc(100dvh-2rem)] gap-0 overflow-y-auto p-0 sm:max-w-lg">
        {abierto ? <Cuerpo usuario={usuario} usuarios={usuarios} alCerrar={alCerrar} /> : null}
      </DialogContent>
    </Dialog>
  )
}

function Cuerpo({ usuario, usuarios, alCerrar }: { usuario: Usuario | null; usuarios: Usuario[]; alCerrar: () => void }) {
  const sesion = useSesion()
  const esYo = usuario?.id === sesion.usuario?.id
  const editando = usuario !== null
  const [nombre, setNombre] = useState(usuario?.nombreCompleto ?? '')
  const [email, setEmail] = useState(usuario?.email ?? '')
  const [contrasena, setContrasena] = useState('')
  const [verContrasena, setVerContrasena] = useState(false)
  const [rol, setRol] = useState<Rol | ''>(usuario?.rol ?? '')
  const [idDoctor, setIdDoctor] = useState<number | null>(usuario?.idDoctor ?? null)
  const [idPaciente, setIdPaciente] = useState<number | null>(usuario?.idPaciente ?? null)
  const [activo, setActivo] = useState(usuario?.activo ?? true)
  const [mostrarErrores, setMostrarErrores] = useState(false)
  const [errorApi, setErrorApi] = useState<unknown>(null)

  const doctores = useListaDoctores()
  const pacientes = usePacientes()
  const crear = useCrearUsuario()
  const editar = useEditarUsuario()
  const enviando = crear.isPending || editar.isPending
  const base = useId()
  const id = (campo: string) => `${base}-${campo}`

  const otros = usuarios.filter((u) => u.id !== usuario?.id)
  const doctorConCuenta = new Set(otros.map((u) => u.idDoctor).filter(Boolean))
  const pacienteConCuenta = new Set(otros.map((u) => u.idPaciente).filter(Boolean))

  const errores: Record<string, string | undefined> = {
    nombre: !nombre.trim() ? 'Escribe el nombre.' : undefined,
    email: editando
      ? undefined
      : !email.trim()
        ? 'Escribe el correo.'
        : !CORREO.test(email.trim())
          ? 'El correo no tiene un formato válido.'
          : undefined,
    contrasena: editando
      ? contrasena
        ? errorContrasena(contrasena)
        : undefined
      : !contrasena
        ? 'Escribe una contraseña.'
        : errorContrasena(contrasena),
    rol: !rol ? 'Elige el rol.' : undefined,
    doctor: rol === 'Medico' && !idDoctor ? 'Elige el doctor de esta cuenta.' : undefined,
    paciente: rol === 'Paciente' && !idPaciente ? 'Elige el paciente de esta cuenta.' : undefined,
  }
  const error = (campo: string) => (mostrarErrores ? errores[campo] : undefined)

  const elegirRol = (nuevo: Rol) => {
    setRol(nuevo)
    if (nuevo !== 'Medico') setIdDoctor(null)
    if (nuevo !== 'Paciente') setIdPaciente(null)
  }

  const enviar = (evento: FormEvent) => {
    evento.preventDefault()
    setMostrarErrores(true)
    setErrorApi(null)
    if (Object.values(errores).some(Boolean) || !rol) return

    const vinculo = {
      idDoctor: rol === 'Medico' ? idDoctor : null,
      idPaciente: rol === 'Paciente' ? idPaciente : null,
    }
    const opciones = {
      onSuccess: () => {
        toast.success(editando ? 'Cuenta actualizada.' : `Cuenta creada para ${nombre.trim()}.`)
        alCerrar()
      },
      onError: setErrorApi,
    }

    if (usuario)
      editar.mutate(
        {
          id: usuario.id,
          datos: {
            nombreCompleto: nombre.trim(),
            rol,
            ...vinculo,
            activo: esYo ? true : activo,
            nuevaContrasena: contrasena || null,
          },
        },
        opciones,
      )
    else
      crear.mutate(
        { email: email.trim(), contrasena, nombreCompleto: nombre.trim(), rol, ...vinculo },
        opciones,
      )
  }

  return (
    <form onSubmit={enviar} noValidate className="grid">
      <DialogHeader className="border-b border-linea p-6">
        <DialogTitle className="font-heading text-lg font-semibold">
          {editando ? 'Editar cuenta' : 'Crear cuenta'}
        </DialogTitle>
        <DialogDescription className="text-pizarra">
          {editando ? `Correo de acceso: ${usuario.email}` : 'La persona entra con este correo y contraseña.'}
        </DialogDescription>
      </DialogHeader>

      <div className="grid gap-5 p-6">
        {errorApi ? <AvisoError error={errorApi} titulo="Revisa los datos de la cuenta:" /> : null}

        <Campo etiqueta="Rol" htmlFor={id('rol')} error={error('rol')} ayuda={rol ? DESCRIPCION_ROL[rol] : undefined}>
          <Select value={rol} onValueChange={(v) => elegirRol(v as Rol)} disabled={esYo}>
            <SelectTrigger id={id('rol')} className="w-full" {...conError(id('rol'), error('rol'))}>
              <SelectValue placeholder="Elige el rol" />
            </SelectTrigger>
            <SelectContent>
              {ROLES.map((r) => (
                <SelectItem key={r} value={r}>
                  {TEXTO_ROL[r]}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </Campo>

        {rol === 'Medico' ? (
          <Campo etiqueta="Doctor" htmlFor={id('doctor')} error={error('doctor')}>
            <Select
              value={idDoctor ? String(idDoctor) : ''}
              onValueChange={(v) => {
                const doctor = doctores.data?.find((d) => d.id === Number(v))
                setIdDoctor(Number(v))
                if (doctor && !nombre.trim()) setNombre(doctor.nombreCompleto)
              }}
            >
              <SelectTrigger id={id('doctor')} className="h-9 w-full" {...conError(id('doctor'), error('doctor'))}>
                <SelectValue placeholder={doctores.isPending ? 'Cargando doctores…' : 'Elige el doctor'} />
              </SelectTrigger>
              <SelectContent>
                {(doctores.data ?? []).map((d) => (
                  <SelectItem key={d.id} value={String(d.id)} disabled={doctorConCuenta.has(d.id)}>
                    {d.nombreCompleto}
                    <span className="text-xs text-pizarra">
                      {doctorConCuenta.has(d.id) ? '· ya tiene cuenta' : `· ${d.nombreEspecialidad}`}
                    </span>
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </Campo>
        ) : null}

        {rol === 'Paciente' ? (
          <Campo etiqueta="Paciente" htmlFor={id('paciente')} error={error('paciente')}>
            <SelectorPaciente
              id={id('paciente')}
              pacientes={(pacientes.data ?? []).filter((p) => !pacienteConCuenta.has(p.id))}
              cargando={pacientes.isPending}
              valor={idPaciente}
              alCambiar={(valor) => {
                const paciente = pacientes.data?.find((p) => p.id === valor)
                setIdPaciente(valor)
                if (paciente && !nombre.trim()) setNombre(`${paciente.nombres} ${paciente.apellidos}`)
              }}
              invalido={!!error('paciente')}
            />
          </Campo>
        ) : null}

        <Campo etiqueta="Nombre" htmlFor={id('nombre')} error={error('nombre')}>
          <Input
            id={id('nombre')}
            value={nombre}
            maxLength={150}
            onChange={(e) => setNombre(e.target.value)}
            {...conError(id('nombre'), error('nombre'))}
          />
        </Campo>

        {!editando ? (
          <Campo etiqueta="Correo" htmlFor={id('email')} error={error('email')}>
            <Input
              id={id('email')}
              type="email"
              autoComplete="off"
              value={email}
              maxLength={100}
              onChange={(e) => setEmail(e.target.value)}
              {...conError(id('email'), error('email'))}
            />
          </Campo>
        ) : null}

        <Campo
          etiqueta={editando ? 'Nueva contraseña' : 'Contraseña'}
          opcional={editando}
          htmlFor={id('contrasena')}
          error={error('contrasena')}
          ayuda={editando ? 'Déjala vacía para conservar la actual.' : 'Mínimo 8 caracteres, con letras y números.'}
        >
          <div className="relative">
            <Input
              id={id('contrasena')}
              type={verContrasena ? 'text' : 'password'}
              autoComplete="new-password"
              value={contrasena}
              maxLength={100}
              onChange={(e) => setContrasena(e.target.value)}
              className="pr-10"
              {...conError(id('contrasena'), error('contrasena'))}
            />
            <Button
              type="button"
              variant="ghost"
              size="icon-sm"
              className="absolute top-1/2 right-1 -translate-y-1/2"
              aria-label={verContrasena ? 'Ocultar contraseña' : 'Mostrar contraseña'}
              onClick={() => setVerContrasena((v) => !v)}
            >
              {verContrasena ? <EyeOff /> : <Eye />}
            </Button>
          </div>
        </Campo>

        {editando ? (
          <div className="flex items-start gap-3 rounded-md border border-linea p-3">
            <Checkbox
              id={id('activo')}
              checked={esYo ? true : activo}
              disabled={esYo}
              onCheckedChange={(v) => setActivo(v === true)}
              className="mt-0.5"
            />
            <div className="grid gap-0.5">
              <Label htmlFor={id('activo')} className="font-medium">
                Cuenta activa
              </Label>
              <p className="text-xs text-pizarra">
                {esYo ? 'No puedes desactivar tu propia cuenta.' : 'Una cuenta inactiva no puede iniciar sesión.'}
              </p>
            </div>
          </div>
        ) : null}
      </div>

      <PieFormulario enviando={enviando} textoEnviar={editando ? 'Guardar cambios' : 'Crear cuenta'} alCancelar={alCerrar} />
    </form>
  )
}

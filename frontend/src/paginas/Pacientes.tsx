import { ChevronRight, UserPlus } from 'lucide-react'
import { useState } from 'react'
import { Link, useNavigate } from 'react-router'
import { EncabezadoPagina } from '@/componentes/EncabezadoPagina'
import { ErrorVista, Vacio } from '@/componentes/EstadosVista'
import { FormularioPaciente } from '@/componentes/pacientes/FormularioPaciente'
import { usePacientes } from '@/componentes/pacientes/consultas'
import {
  Buscador,
  CONTENEDOR_TABLA,
  ENCABEZADO_TABLA,
  EsqueletoTabla,
  InsigniaActivo,
  Paginacion,
  paginar,
} from '@/componentes/Tabla'
import { Button } from '@/components/ui/button'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { normalizar, numeroExpediente, TEXTO_DOCUMENTO, textoEdad } from '@/lib/catalogos'
import { useSesion } from '@/sesion/SesionContext'

/**
 * Pacientes (specs/frontend-pantallas.md, "Pacientes"): tabla con búsqueda por nombre,
 * documento o número de expediente, y paginación en el cliente.
 */
export default function PaginaPacientes() {
  const { usuario } = useSesion()
  const puedeRegistrar = usuario?.rol === 'Administrador' || usuario?.rol === 'Recepcionista'
  const navegar = useNavigate()
  const consulta = usePacientes()
  const [busqueda, setBusqueda] = useState('')
  const [pagina, setPagina] = useState(1)
  const [registrando, setRegistrando] = useState(false)

  const termino = normalizar(busqueda.trim())
  const filtrados = (consulta.data ?? []).filter(
    (p) =>
      !termino ||
      normalizar(`${p.nombres} ${p.apellidos} ${p.numeroDocumento} ${numeroExpediente(p.id)}`).includes(termino),
  )
  const visibles = paginar(filtrados, pagina)

  return (
    <div className="grid gap-6">
      <EncabezadoPagina
        titulo="Pacientes"
        descripcion={
          consulta.data
            ? `${consulta.data.length} pacientes registrados`
            : 'Expedientes de los pacientes de la clínica'
        }
        accion={
          puedeRegistrar ? (
            <Button onClick={() => setRegistrando(true)}>
              <UserPlus aria-hidden="true" />
              Registrar paciente
            </Button>
          ) : null
        }
      />

      <Buscador
        valor={busqueda}
        alCambiar={(valor) => {
          setBusqueda(valor)
          setPagina(1)
        }}
        etiqueta="Buscar por nombre, documento o expediente"
      />

      {consulta.isPending ? (
        <EsqueletoTabla columnas={5} />
      ) : consulta.isError ? (
        <ErrorVista error={consulta.error} alReintentar={() => consulta.refetch()} />
      ) : consulta.data.length === 0 ? (
        <Vacio
          mensaje="Todavía no hay pacientes registrados."
          accion={
            puedeRegistrar ? (
              <Button variant="outline" onClick={() => setRegistrando(true)}>
                Registrar el primero
              </Button>
            ) : null
          }
        />
      ) : filtrados.length === 0 ? (
        <Vacio mensaje={`Ningún paciente coincide con “${busqueda}”.`} />
      ) : (
        <div className="grid gap-3">
          <div className={CONTENEDOR_TABLA}>
            <Table>
              <TableHeader>
                <TableRow className={ENCABEZADO_TABLA}>
                  <TableHead className="pl-4">Paciente</TableHead>
                  <TableHead className="hidden md:table-cell">Documento</TableHead>
                  <TableHead className="hidden sm:table-cell">Edad</TableHead>
                  <TableHead className="hidden lg:table-cell">Teléfono</TableHead>
                  <TableHead>Estado</TableHead>
                  <TableHead className="w-10">
                    <span className="sr-only">Abrir</span>
                  </TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {visibles.map((p) => (
                  <TableRow
                    key={p.id}
                    className="cursor-pointer"
                    onClick={() => navegar(`/pacientes/${p.id}`)}
                  >
                    <TableCell className="py-3 pl-4">
                      <Link
                        to={`/pacientes/${p.id}`}
                        onClick={(e) => e.stopPropagation()}
                        className="font-medium text-tinta hover:text-marea focus-visible:text-marea"
                      >
                        {p.nombres} {p.apellidos}
                      </Link>
                      <p className="cifras text-xs text-pizarra">
                        {numeroExpediente(p.id)}
                        <span className="md:hidden"> · {p.numeroDocumento}</span>
                      </p>
                    </TableCell>
                    <TableCell className="hidden md:table-cell">
                      <span className="text-pizarra">{TEXTO_DOCUMENTO[p.tipoDocumento]}</span>{' '}
                      <span className="cifras">{p.numeroDocumento}</span>
                    </TableCell>
                    <TableCell className="cifras hidden sm:table-cell">{textoEdad(p.fechaNacimiento)}</TableCell>
                    <TableCell className="cifras hidden text-pizarra lg:table-cell">{p.telefono || '—'}</TableCell>
                    <TableCell>
                      <InsigniaActivo activo={p.activo} />
                    </TableCell>
                    <TableCell className="pr-4 text-pizarra">
                      <ChevronRight className="size-4" aria-hidden="true" />
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>
          <Paginacion pagina={pagina} total={filtrados.length} alCambiar={setPagina} nombre="pacientes" />
        </div>
      )}

      <FormularioPaciente
        abierto={registrando}
        alCerrar={() => setRegistrando(false)}
        alGuardar={(nuevo) => navegar(`/pacientes/${nuevo.id}`)}
      />
    </div>
  )
}

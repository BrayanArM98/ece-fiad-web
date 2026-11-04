import { Check, ChevronsUpDown, Search } from 'lucide-react'
import { useId, useState, type KeyboardEvent } from 'react'
import { Button } from '@/components/ui/button'
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover'
import { normalizar } from '@/lib/catalogos'
import type { Paciente } from '@/lib/tipos'
import { cn } from '@/lib/utils'

const MAXIMO_RESULTADOS = 50

export const nombrePaciente = (p: Paciente) => `${p.nombres} ${p.apellidos}`

/**
 * Selector de paciente con búsqueda por nombre o número de documento.
 * Con el teclado: escribir para filtrar, flechas para moverse y Enter para elegir.
 */
export function SelectorPaciente({
  id,
  pacientes,
  cargando,
  valor,
  alCambiar,
  invalido,
  descripcionId,
}: {
  id: string
  pacientes: Paciente[]
  cargando: boolean
  valor: number | null
  alCambiar: (idPaciente: number) => void
  invalido?: boolean
  descripcionId?: string
}) {
  const [abierto, setAbierto] = useState(false)
  const [busqueda, setBusqueda] = useState('')
  const [activo, setActivo] = useState(0)
  const idLista = useId()

  const seleccionado = pacientes.find((p) => p.id === valor)
  const termino = normalizar(busqueda.trim())
  const coincidencias = termino
    ? pacientes.filter((p) => normalizar(`${p.nombres} ${p.apellidos} ${p.numeroDocumento}`).includes(termino))
    : pacientes
  const visibles = coincidencias.slice(0, MAXIMO_RESULTADOS)

  const elegir = (paciente: Paciente) => {
    alCambiar(paciente.id)
    setAbierto(false)
    setBusqueda('')
  }

  const alTeclear = (evento: KeyboardEvent<HTMLInputElement>) => {
    if (evento.key === 'ArrowDown') {
      evento.preventDefault()
      setActivo((i) => Math.min(i + 1, visibles.length - 1))
    } else if (evento.key === 'ArrowUp') {
      evento.preventDefault()
      setActivo((i) => Math.max(i - 1, 0))
    } else if (evento.key === 'Enter') {
      evento.preventDefault()
      if (visibles[activo]) elegir(visibles[activo])
    }
  }

  return (
    <Popover
      open={abierto}
      onOpenChange={(siguiente) => {
        setAbierto(siguiente)
        if (siguiente) setActivo(Math.max(0, visibles.findIndex((p) => p.id === valor)))
      }}
    >
      <PopoverTrigger asChild>
        <Button
          id={id}
          type="button"
          variant="outline"
          role="combobox"
          aria-expanded={abierto}
          aria-controls={idLista}
          aria-invalid={invalido || undefined}
          aria-describedby={descripcionId}
          disabled={cargando}
          className="h-9 w-full justify-between px-3 font-normal"
        >
          {seleccionado ? (
            <span className="truncate">
              {nombrePaciente(seleccionado)}
              <span className="ml-2 text-pizarra">{seleccionado.numeroDocumento}</span>
            </span>
          ) : (
            <span className="text-pizarra">{cargando ? 'Cargando pacientes…' : 'Elige un paciente'}</span>
          )}
          <ChevronsUpDown className="text-pizarra" aria-hidden="true" />
        </Button>
      </PopoverTrigger>
      <PopoverContent align="start" className="w-(--radix-popover-trigger-width) min-w-72 gap-0 p-0">
        <div className="flex items-center gap-2 border-b border-linea px-3">
          <Search className="size-4 text-pizarra" aria-hidden="true" />
          <input
            autoFocus
            value={busqueda}
            onChange={(e) => {
              setBusqueda(e.target.value)
              setActivo(0)
            }}
            onKeyDown={alTeclear}
            placeholder="Nombre o número de documento"
            aria-label="Buscar paciente"
            aria-controls={idLista}
            aria-activedescendant={visibles[activo] ? `${idLista}-${visibles[activo].id}` : undefined}
            className="h-10 flex-1 bg-transparent outline-none placeholder:text-pizarra"
          />
        </div>
        <ul id={idLista} role="listbox" aria-label="Pacientes" className="max-h-64 overflow-y-auto p-1">
          {visibles.length === 0 ? (
            <li className="px-3 py-6 text-center text-pizarra">Ningún paciente coincide con la búsqueda.</li>
          ) : (
            visibles.map((paciente, indice) => (
              <li
                key={paciente.id}
                id={`${idLista}-${paciente.id}`}
                role="option"
                aria-selected={paciente.id === valor}
                onMouseEnter={() => setActivo(indice)}
                onMouseDown={(e) => e.preventDefault()}
                onClick={() => elegir(paciente)}
                className={cn(
                  'flex cursor-pointer items-center gap-2 rounded-md px-2 py-1.5',
                  indice === activo && 'bg-muted',
                )}
              >
                <Check
                  className={cn('size-4 shrink-0 text-marea', paciente.id !== valor && 'invisible')}
                  aria-hidden="true"
                />
                <span className="min-w-0 flex-1 truncate">{nombrePaciente(paciente)}</span>
                <span className="cifras shrink-0 text-xs text-pizarra">{paciente.numeroDocumento}</span>
              </li>
            ))
          )}
        </ul>
        {coincidencias.length > MAXIMO_RESULTADOS ? (
          <p className="border-t border-linea px-3 py-2 text-xs text-pizarra">
            Hay {coincidencias.length - MAXIMO_RESULTADOS} pacientes más: sigue escribiendo para acotar.
          </p>
        ) : null}
      </PopoverContent>
    </Popover>
  )
}

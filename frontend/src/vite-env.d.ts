/// <reference types="vite/client" />

interface ImportMetaEnv {
  /** Dirección de la API ECE-FIAD, por ejemplo http://localhost:5200 */
  readonly VITE_API_URL?: string
}

interface ImportMeta {
  readonly env: ImportMetaEnv
}

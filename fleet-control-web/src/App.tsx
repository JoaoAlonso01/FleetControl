import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import Layout from './components/Layout'
import VeiculosPage from './pages/VeiculosPage'
import UsuariosPage from './pages/UsuariosPage'
import ManutencoesPage from './pages/ManutencoesPage'
import MovimentacoesPage from './pages/MovimentacoesPage'

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      retry: 1,
      staleTime: 30_000,
    },
  },
})

export default function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        <Routes>
          <Route element={<Layout />}>
            <Route index element={<Navigate to="/veiculos" replace />} />
            <Route path="/veiculos" element={<VeiculosPage />} />
            <Route path="/usuarios" element={<UsuariosPage />} />
            <Route path="/manutencoes" element={<ManutencoesPage />} />
            <Route path="/movimentacoes" element={<MovimentacoesPage />} />
          </Route>
        </Routes>
      </BrowserRouter>
    </QueryClientProvider>
  )
}


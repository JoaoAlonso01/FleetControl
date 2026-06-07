import { NavLink } from 'react-router-dom'
import { Car, Users, Wrench, ArrowLeftRight } from 'lucide-react'

const navItems = [
  { to: '/veiculos', label: 'Veículos', icon: Car },
  { to: '/usuarios', label: 'Usuários', icon: Users },
  { to: '/manutencoes', label: 'Manutenção', icon: Wrench },
  { to: '/movimentacoes', label: 'Check-in/Out', icon: ArrowLeftRight },
]

export default function Sidebar() {
  return (
    <aside className="w-64 min-h-screen bg-slate-900 text-white flex flex-col">
      <div className="p-6 border-b border-slate-700">
        <div className="flex items-center gap-3">
          <Car className="text-blue-400" size={28} />
          <div>
            <p className="font-bold text-lg leading-tight">Fleet Control</p>
            <p className="text-xs text-slate-400">Controle de Frotas</p>
          </div>
        </div>
      </div>
      <nav className="flex-1 p-4 space-y-1">
        {navItems.map(({ to, label, icon: Icon }) => (
          <NavLink
            key={to}
            to={to}
            className={({ isActive }) =>
              `flex items-center gap-3 px-4 py-3 rounded-lg text-sm font-medium transition-colors ${
                isActive
                  ? 'bg-blue-600 text-white'
                  : 'text-slate-300 hover:bg-slate-800 hover:text-white'
              }`
            }
          >
            <Icon size={18} />
            {label}
          </NavLink>
        ))}
      </nav>
      <div className="p-4 border-t border-slate-700 text-xs text-slate-500 text-center">
        Fleet Control v1.0
      </div>
    </aside>
  )
}

import {ThemeSwitch} from './action.set-theme'

export function meta() {
  return [
    {title: 'BillSplit'},
    {name: 'description', content: 'Welcome BillSplit'},
  ]
}

export default function Index() {
  return (
    <div>
      <ThemeSwitch />
      <h1>Hello world!</h1>
    </div>
  )
}

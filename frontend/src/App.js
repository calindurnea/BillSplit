import Expenses from "./components/Expenses/Expenses";
import NewExpense from "./components/NewExpense/NewExpense";

const App = () => {
  const expenses = [
    { title: "Test1", amount: 10, date: new Date(2022, 1, 1) },
    { title: "Test2", amount: 20, date: new Date(2023, 1, 1) },
    { title: "Test3", amount: 30, date: new Date(2024, 1, 1) },
    { title: "Test4", amount: 40, date: new Date(2022, 1, 1) },
  ];
  return (
    <div>
      <NewExpense />
      <Expenses items={expenses} />
    </div>
  );
};

export default App;

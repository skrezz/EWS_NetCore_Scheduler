import React from 'react';
import {DevScheduler} from './components/Scheduler';
import { QueryClient, QueryClientProvider } from "react-query";

const queryClient = new QueryClient();

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <DevScheduler/>
    </QueryClientProvider>
  );
}

export default App;

import { render, screen, waitFor } from "@testing-library/react";
import axios from "axios";
import App from "./App";

jest.mock("axios"); // or any other library for making API calls

describe("App", () => {
  it("displays data from the API", async () => {
    const currentDate = new Date();
    const today = new Date(currentDate.getFullYear(),currentDate.getMonth(),currentDate.getDate());
    const startDate = new Date(today.getTime() + (9 * 60 * 60 * 1000));
    const endDate = new Date(today.getTime() + (10 * 60 * 60 * 1000));
    const mockData = [{
      startDate: startDate.toISOString(),
      endDate: endDate.toISOString(),
      title: "Meeting",
      type: "private",
    }];

    //@ts-ignore
    axios.get.mockResolvedValueOnce({ data: mockData });
    const { getByText } = render(<App />);
    await waitFor(() => expect(getByText("Meeting")).toBeInTheDocument());
   
  });
});

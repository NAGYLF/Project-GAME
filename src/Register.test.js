import React from "react";
import { render, screen, fireEvent } from "@testing-library/react";
import Register from "./Register";

const mockNavigate = jest.fn();
const mockShowAlert = jest.fn();
const mockLogin = jest.fn();
const mockAdminCode = jest.fn();

jest.mock("react-router-dom", () => ({
  ...jest.requireActual("react-router-dom"),
  useNavigate: () => mockNavigate,
  useLocation: () => ({ pathname: "/register" }),
}));

describe("Regisztrációs űrlap validációk", () => {
  beforeEach(() => {
    render(
      <Register
        language="hu"
        code="admin123"
        login={mockLogin}
        admincode={mockAdminCode}
        showAlert={mockShowAlert}
      />
    );
  });

  test("Hibát jelez, ha a két jelszó nem egyezik", () => {
    fireEvent.change(screen.getByLabelText(/Felhasználónév/i), {
      target: { value: "TesztUser" },
    });
    fireEvent.change(screen.getByLabelText(/Email/i), {
      target: { value: "teszt@example.com" },
    });
    fireEvent.change(screen.getByLabelText("Jelszó"), {
      target: { value: "jelszo123" },
    });
    fireEvent.change(screen.getByLabelText("Jelszó újra"), {
      target: { value: "masikjelszo" },
    });

    fireEvent.submit(screen.getByRole("button", { name: "Regisztráció" }));

    expect(mockShowAlert).toHaveBeenCalledWith("A két jelszó nem egyezik!", "error");
  });

  test("Nem történik regisztráció, ha a mezők üresek", () => {
    fireEvent.submit(screen.getByRole("button", { name: "Regisztráció" }));
    expect(mockShowAlert).not.toHaveBeenCalled();
  });

  test("Rossz formátumú email nem valid", () => {
    const emailInput = screen.getByLabelText(/Email/i);
    fireEvent.change(emailInput, { target: { value: "hibasformatum" } });

    expect(emailInput.validity.valid).toBe(false);
  });
});

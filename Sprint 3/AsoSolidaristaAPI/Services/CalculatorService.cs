using AsoSolidaristaAPI.Models.Requests;
using System;

public class CalculatorService
{
    public (decimal amount, string formula) CalculateFee(CalculationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.AssociationName))
            throw new ArgumentException("El nombre de la asociación no puede estar vacío");

        // Obtener la primera letra en mayúscula
        char firstLetter = char.ToUpper(request.AssociationName.Trim()[0]);

        // Definir rangos por posición en el abecedario
        string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        int index = alphabet.IndexOf(firstLetter);

        if (index == -1)
            throw new ArgumentException("La primera letra de la asociación no es válida");

        decimal percentage;
        string formula;

        if (index <= 4) // A-E
        {
            percentage = 3.0m;
            formula = "Letra entre A-E = 3%";
        }
        else if (index <= 9) // F-J
        {
            percentage = 3.5m;
            formula = "Letra entre F-J = 3.5%";
        }
        else if (index <= 14) // K-O
        {
            percentage = 4.0m;
            formula = "Letra entre K-O = 4%";
        }
        else if (index <= 18) // P-S
        {
            percentage = 4.5m;
            formula = "Letra entre P-S = 4.5%";
        }
        else // T-Z
        {
            percentage = 5.0m;
            formula = "Letra entre T-Z = 5%";
        }

        decimal amount = request.EmployeeSalary * percentage / 100;
        return (amount, formula);
    }
}


// http://localhost:5096/api/public/calculator/calculate

// Link para correr la cosa

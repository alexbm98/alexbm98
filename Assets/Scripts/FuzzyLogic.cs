using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuzzyLogic : MonoBehaviour
{
    private float saludCercaMuerte = 0;
    private float saludMedia = 0;
    private float saludSaludable = 0;

    private float staminaMuyCansado = 0;
    private float staminaAlgoCansado = 0;
    private float staminaPocoCansado = 0;

    private float gradoSoltarPocionSalud = 0;
    private float gradoSoltarPocionStamina = 0;
    private float gradoNoSoltarPocion = 0;

    // Update is called once per frame
    void Update()
    {
        float salud = PlayerManager.Instance.currentHealth;
        float stamina = PlayerManager.Instance.currentStamina;

        saludCercaMuerte = fPertenenciaTriangular(salud, -35, 35, 0);
        saludMedia = fPertenenciaTrapezoidal(salud, 30, 75, 45, 60);
        saludSaludable = fPertenenciaTriangular(salud, 70, 130, 100);

        staminaMuyCansado = fPertenenciaTriangular(stamina, -35, 35, 0);
        staminaAlgoCansado = fPertenenciaTrapezoidal(stamina, 30, 75, 45, 60);
        staminaPocoCansado = fPertenenciaTriangular(stamina, 70, 130, 100);

        //Suelta poción de salud cuando: saludCercaMuerte || saludMedia
        gradoSoltarPocionSalud = fOR(saludCercaMuerte, saludMedia);

        //Suelta poción de stamina cuando: (staminaAlgoCansado || staminaMuyCansado) && saludSaludable
        gradoSoltarPocionStamina = fAND(fOR(staminaAlgoCansado, staminaMuyCansado), saludSaludable);

        //No suelta poción cuando: saludSaludable && staminaPocoCansado
        gradoNoSoltarPocion = fAND(saludSaludable, staminaPocoCansado);

        if (gradoSoltarPocionSalud > gradoSoltarPocionStamina && gradoSoltarPocionSalud > gradoNoSoltarPocion)
        {
            DroneController.Instance.pocionASoltar = "health";
        }
        else
        {
            if (gradoSoltarPocionStamina > gradoSoltarPocionSalud && gradoSoltarPocionStamina > gradoNoSoltarPocion)
            {
                DroneController.Instance.pocionASoltar = "stamina";
            }
            else
            {
                if (gradoNoSoltarPocion > gradoSoltarPocionSalud && gradoNoSoltarPocion > gradoSoltarPocionStamina)
                {
                    DroneController.Instance.pocionASoltar = "none";
                }
            }
        }
    }

    private float fPertenenciaTriangular(float x, float limiteInf, float limiteSup, float m)
    {
        if (x <= limiteInf || x >= limiteSup)
        {
            return 0;
        }

        if (x > limiteInf && x <= m)
        {
            return (x - limiteInf) / (m - limiteInf);
        }

        if (x > m && x < limiteSup)
        {
            return (limiteSup - x) / (limiteSup - m);
        }

        return 0;
    }

    private float fPertenenciaTrapezoidal(float x, float limiteInf, float limiteSup, float limiteSopInf, float limiteSopSup)
    {
        if (x < limiteInf || x > limiteSup)
        {
            return 0;
        }

        if (x >= limiteInf && x <= limiteSopInf)
        {
            return (x - limiteInf) / (limiteSopInf - limiteInf);
        }

        if (x >= limiteSopInf && x <= limiteSopSup)
        {
            return 1;
        }

        if (x >= limiteSopSup && x <= limiteSup)
        {
            return (limiteSup - x) / (limiteSup - limiteSopSup);
        }

        return 0;
    }

    private float fOR(float x, float y)
    {
        return Mathf.Max(x, y);
    }

    private float fAND(float x, float y)
    {
        return Mathf.Min(x, y);
    }
}

//-------------------------------------------------------------------------
// <copyright file="Recipe.cs" company="Universidad Católica del Uruguay">
// Copyright (c) Programación II. Derechos reservados.
// </copyright>
//-------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using System.Xml.XPath;

namespace Full_GRASP_And_SOLID
{
    public class Recipe : IRecipeContent // Modificado por DIP
    {
        // Cambiado por OCP
        private IList<BaseStep> steps = new List<BaseStep>();

        public Product FinalProduct { get; set; }
        public bool Cooked {get; private set;}

        // Agregado por Creator
        public void AddStep(Product input, double quantity, Equipment equipment, int time)
        {
            Step step = new Step(input, quantity, equipment, time);
            this.steps.Add(step);
        }

        // Agregado por OCP y Creator
        public void AddStep(string description, int time)
        {
            WaitStep step = new WaitStep(description, time);
            this.steps.Add(step);
        }

        public void RemoveStep(BaseStep step)
        {
            this.steps.Remove(step);
        }

        // Agregado por SRP
        public string GetTextToPrint()
        {
            string result = $"Receta de {this.FinalProduct.Description}:\n";
            foreach (BaseStep step in this.steps)
            {
                result = result + step.GetTextToPrint() + "\n";
            }

            // Agregado por Expert
            result = result + $"Costo de producción: {this.GetProductionCost()}";

            return result;
        }

        // Agregado por Expert
        public double GetProductionCost()
        {
            double result = 0;

            foreach (BaseStep step in this.steps)
            {
                result = result + step.GetStepCost();
            }

            return result;
        }

        // devolver Tiempo
        public int GetCookTime()
        {
            int cookTime = 0;
            foreach (BaseStep step in this.steps)
            {
                cookTime += step.Time;
            }
            return cookTime; 
        }

        public void Cook()
        {
            this.Cooked = false;

            RecipeAdapter timerClient = new RecipeAdapter(this);
            int overallTime = this.GetCookTime();

            CountdownTimer timer = new CountdownTimer();
            timer.Register(overallTime,timerClient);
        }

        public class RecipeAdapter : TimerClient
        {
            private Recipe recipe;
            public RecipeAdapter(Recipe recipe)
            {
                this.recipe = recipe;
            }
            public void TimeOut()
            {
                this.recipe.Cooked = true;
            }
        }
    }
}
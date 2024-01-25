using System.Collections.Generic;

namespace SpaceInvaders.Models
{
    public class InvaderFormation
    {
        public List<Invader> Invaders { get; private set; }

        public InvaderFormation()
        {
            Invaders = new List<Invader>();
            InitializeFormation();
        }

        private void InitializeFormation()
        {
            // Adicione os aliens à formação com suas posições específicas
            // Aqui está um exemplo com uma formação simples
            for (int row = 0; row < 5; row++)
            {
                
                for (int col = 0; col < 11; col++)
                {
                    double x = col;  // Ajuste conforme necessário
                    double y = row;  // Ajuste conforme necessário
                    Invaders.Add(new Invader(col,row,x, y));
                }
            }
        }
    }
}
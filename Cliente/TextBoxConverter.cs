#region Usings

using System;
using System.Globalization;
using System.Windows.Data;

#endregion

namespace Cliente {

    /// <summary>
    ///     Classe que faz conversão de valores ao utilizar o bind
    /// </summary>
    internal class TextBoxConverter : IValueConverter {

        /// <summary>
        ///     Método que vai cuidar do tamanho da área de texto do textblock da janela de login
        /// </summary>
        /// <param name="value">Parametro recebido do bind Width do retangulo</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>Valor alterado</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            // Seta valor do width com o tamanho recebido (tamanho do retangulo) menos a diferença
            var val = (int) (double) value - 80;

            // Retorna o valor corrigido
            return val;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            // Deveria fazer a conversão inversa, mas a mesma não é necessária
            throw new NotImplementedException();
        }

    }

}
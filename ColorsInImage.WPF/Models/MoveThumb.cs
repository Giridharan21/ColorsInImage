using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ColorsInImage.WPF.Models
{
    public class MoveThumb : Thumb
    {
        public MoveThumb()
        {
            DragDelta += new DragDeltaEventHandler(MoveThumb_Drag);
        }
        private void MoveThumb_Drag(object sender, DragDeltaEventArgs e)
        {
            Control item = this.DataContext as Control;
            if(item != null)
            {
                var left = Canvas.GetLeft(item);
                var top = Canvas.GetTop(item);

                Canvas.SetLeft(item, left + e.HorizontalChange);
                Canvas.SetTop(item, top + e.VerticalChange);
            }
        } 
    }

}

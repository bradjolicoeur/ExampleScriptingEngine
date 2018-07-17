namespace UserScript
{
    using System;
    using System.Text;
    public class RunScript
    {
        //Properties
        private int VehicleType { get; set; }

        public object Eval()
        {
            object Result = null;
            //Code 
            Result = (VehicleType == 90);
            return Result;
        }
    }
}
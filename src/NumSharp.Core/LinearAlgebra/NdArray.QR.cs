using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using NumSharp.Core.Shared;

namespace NumSharp.Core
{
    public partial class NDArray
    {
        public (NDArray,NDArray) qr()
        {
            var a = (double[]) this.Storage.GetData<double>().Clone();

            int m = this.Storage.Shape.Dimensions[0];
            int n = this.Storage.Shape.Dimensions[1];

            int lda = m;

            double[] tau = new double[ Math.Min(m,n) ];
            double[] work = new double[ Math.Max(m,n) ];

            int lwork = m;

            int info = 0;

            LAPACK.dgeqrf_(ref m,ref n, a ,ref lda, tau, work,ref lwork,ref info);

            double[] RDouble = new double[n*n];

            for(int idx = 0; idx < n; idx++)
                for(int jdx = idx;jdx < n;jdx++)
                    RDouble[idx+jdx * n] = a[idx+jdx*n];


            var R = new NDArray(typeof(double),n,n);

            R.Storage.SetData(RDouble);

            int k = tau.Length;

            LAPACK.dorgqr_(ref m, ref n,ref k ,a, ref lda, tau, work, ref lwork,ref info);

            var Q = new NDArray(typeof(double),tau.Length,tau.Length);

            Q.Storage.Allocate(Q.Storage.DType,Q.Storage.Shape,2);
            Q.Storage.SetData(a);
            Q.Storage.ChangeTensorLayout(1);

            return (Q,R);
        }
    }
}
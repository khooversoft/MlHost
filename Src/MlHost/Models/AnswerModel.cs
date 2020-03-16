﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MlHost.Models
{
    public class AnswerModel
    {
        public string? Answer { get; set; }

        public int End { get; set; }

        public double Score { get; set; }

        public int Start { get; set; }
    }
}
﻿using System;
namespace Sophie.Model
{
    public class AWSOptions
    {
        public string Profile { get; set; }
        public string Region { get; set; }

        public string BucketName { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
    }
}

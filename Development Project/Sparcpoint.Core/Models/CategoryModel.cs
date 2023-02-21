using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Models
{
    public class CategoryModel
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        //EVAL: this is how EF prefers and am in habit of doing for FK
        public int ParentCategoryId { get; set; }
        public CategoryModel ParentCategory { get; set; }
        //EVAL: Don't need CreatedTimestamp on model, it'll be auto generated on insert


        //EVAL: I firmly believe when designing the system, it should start at the Database.
        //EVAL: Solid foundation for clean data in and consistency through to domain (database, entities, and to model) makes maintaining and troubleshooting easier in the long run
        //EVAL: That being said, having InstanceId be the PK on multiple tables will only add to confusion and frustration to anyone coming in behind the developer... like this test.
        //EVAL: I am adding the columns from the database scripts I see, but am leaving the model/entity structure as desired. I probably won't have time to adjust scripts for my preferences for this test.

    //  [InstanceId] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	//  [Name] VARCHAR(64) NOT NULL,
    //  [Description] VARCHAR(256) NOT NULL,
    //  [CreatedTimestamp] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(), 
    }
}

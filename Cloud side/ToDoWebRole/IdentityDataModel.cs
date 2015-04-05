using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.StorageClient;

namespace ToDoWebRole
{
    public class IdentityDataModel : TableServiceEntity
    {
            public IdentityDataModel(Guid partitionKey, string rowKey)
                : base(partitionKey.ToString(), rowKey)
            {
                //Obviously, not generally a good pattern, but since we know pk is GUID...
                this.Id_new = partitionKey;
            }

            public IdentityDataModel()
                : this(Guid.NewGuid(), String.Empty)
            {
            }

            public IdentityDataModel(User TheUser)
            {
                this.Id_new = TheUser.Id;
                this.PartitionKey = this.Id_new.ToString();
                this.RowKey = String.Empty;
                this.UserId = TheUser.UserID;
                this.GroupId = TheUser.GroupID;
                this.Password = TheUser.Password;
                this.GroupPass = TheUser.GroupPass;
            }

            public Guid Id_new { get;set; }
            public string UserId { get; set; }
            public string GroupId { get; set; }
            public string Password { get; set; }
            public string GroupPass { get; set; }
           // public string longitude { get; set; }
           // public string Type { get; set; }
            //public string Predecessor { get; set; }

            /* internal void Add(ToDoDataModel t)
             {
                 t.Parent = this.ID;
             }

             internal void Follows(ToDoDataModel td)
             {
                 this.Predecessor = td.ID;
             }*/

            public readonly static Guid ROOT_ID = new Guid("11111111-1111-1111-1111-111111111111");
    }
 }

namespace Vidly.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeEquationForAvilabiltyStock : DbMigration
    {
        public override void Up()
        {
            Sql("UPDATE Movies SET NumberAvailabe=NumberInStock");
        }

        public override void Down()
        {
        }
    }
}

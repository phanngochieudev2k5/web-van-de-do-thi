using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebDoThi.Migrations
{
    /// <inheritdoc />
    public partial class AddPhoneNumberToReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_Badges",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    image_url = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    required_reports = table.Column<int>(type: "int", nullable: true, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tbl_Badg__3213E83F73897F8C", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    icon = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tbl_Cate__3213E83F70A4B246", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Departments",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    contact_email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tbl_Depa__3213E83FB93E69E6", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Rewards",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    point_cost = table.Column<int>(type: "int", nullable: false),
                    stock_quantity = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    is_active = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tbl_Rewa__3213E83F91A9FAF1", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tbl_Role__3213E83F339C478A", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Menus",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    url = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    icon = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    parent_id = table.Column<int>(type: "int", nullable: true),
                    display_order = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    required_role_id = table.Column<int>(type: "int", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tbl_Menu__3213E83F7CB44E24", x => x.id);
                    table.ForeignKey(
                        name: "FK__tbl_Menus__paren__5EBF139D",
                        column: x => x.parent_id,
                        principalTable: "tbl_Menus",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__tbl_Menus__requi__5FB337D6",
                        column: x => x.required_role_id,
                        principalTable: "tbl_Roles",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "tbl_Users",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    role_id = table.Column<int>(type: "int", nullable: false),
                    full_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    password_hash = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    trust_points = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    status = table.Column<byte>(type: "tinyint", nullable: true, defaultValue: (byte)1),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tbl_User__3213E83F14787C0A", x => x.id);
                    table.ForeignKey(
                        name: "FK__tbl_Users__role___59FA5E80",
                        column: x => x.role_id,
                        principalTable: "tbl_Roles",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "tbl_Reports",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    category_id = table.Column<int>(type: "int", nullable: false),
                    department_id = table.Column<int>(type: "int", nullable: true),
                    title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    severity = table.Column<byte>(type: "tinyint", nullable: true, defaultValue: (byte)1),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    status = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true, defaultValue: "PENDING"),
                    ward_code = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    latitude = table.Column<decimal>(type: "decimal(10,8)", nullable: true),
                    longitude = table.Column<decimal>(type: "decimal(11,8)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    resolved_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tbl_Repo__3213E83F5697BCD2", x => x.id);
                    table.ForeignKey(
                        name: "FK__tbl_Repor__categ__71D1E811",
                        column: x => x.category_id,
                        principalTable: "tbl_Categories",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__tbl_Repor__depar__72C60C4A",
                        column: x => x.department_id,
                        principalTable: "tbl_Departments",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__tbl_Repor__user___70DDC3D8",
                        column: x => x.user_id,
                        principalTable: "tbl_Users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "tbl_User_Badges",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false),
                    badge_id = table.Column<int>(type: "int", nullable: false),
                    earned_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tbl_User__C7C7BE6AD844F7E2", x => new { x.user_id, x.badge_id });
                    table.ForeignKey(
                        name: "FK__tbl_User___badge__6477ECF3",
                        column: x => x.badge_id,
                        principalTable: "tbl_Badges",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__tbl_User___user___6383C8BA",
                        column: x => x.user_id,
                        principalTable: "tbl_Users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "tbl_User_Rewards",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    reward_id = table.Column<int>(type: "int", nullable: false),
                    redeemed_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    voucher_code = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tbl_User__3213E83FA3034873", x => x.id);
                    table.ForeignKey(
                        name: "FK__tbl_User___rewar__6A30C649",
                        column: x => x.reward_id,
                        principalTable: "tbl_Rewards",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__tbl_User___user___693CA210",
                        column: x => x.user_id,
                        principalTable: "tbl_Users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "tbl_Comments",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    report_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tbl_Comm__3213E83FFB9ADDB2", x => x.id);
                    table.ForeignKey(
                        name: "FK__tbl_Comme__repor__7E37BEF6",
                        column: x => x.report_id,
                        principalTable: "tbl_Reports",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__tbl_Comme__user___7F2BE32F",
                        column: x => x.user_id,
                        principalTable: "tbl_Users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "tbl_Interactions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    report_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    type = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tbl_Inte__3213E83F59F26B79", x => x.id);
                    table.ForeignKey(
                        name: "FK__tbl_Inter__repor__797309D9",
                        column: x => x.report_id,
                        principalTable: "tbl_Reports",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__tbl_Inter__user___7A672E12",
                        column: x => x.user_id,
                        principalTable: "tbl_Users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "tbl_Report_Images",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    report_id = table.Column<int>(type: "int", nullable: false),
                    image_url = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tbl_Repo__3213E83FB95F5BD9", x => x.id);
                    table.ForeignKey(
                        name: "FK__tbl_Repor__repor__75A278F5",
                        column: x => x.report_id,
                        principalTable: "tbl_Reports",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Comments_report_id",
                table: "tbl_Comments",
                column: "report_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Comments_user_id",
                table: "tbl_Comments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Interactions_report_id",
                table: "tbl_Interactions",
                column: "report_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Interactions_user_id",
                table: "tbl_Interactions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Menus_parent_id",
                table: "tbl_Menus",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Menus_required_role_id",
                table: "tbl_Menus",
                column: "required_role_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Report_Images_report_id",
                table: "tbl_Report_Images",
                column: "report_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Reports_category_id",
                table: "tbl_Reports",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Reports_department_id",
                table: "tbl_Reports",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Reports_user_id",
                table: "tbl_Reports",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_User_Badges_badge_id",
                table: "tbl_User_Badges",
                column: "badge_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_User_Rewards_reward_id",
                table: "tbl_User_Rewards",
                column: "reward_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_User_Rewards_user_id",
                table: "tbl_User_Rewards",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "UQ__tbl_User__217310695608E43D",
                table: "tbl_User_Rewards",
                column: "voucher_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Users_role_id",
                table: "tbl_Users",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "UQ__tbl_User__AB6E6164C70A20BC",
                table: "tbl_Users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_Comments");

            migrationBuilder.DropTable(
                name: "tbl_Interactions");

            migrationBuilder.DropTable(
                name: "tbl_Menus");

            migrationBuilder.DropTable(
                name: "tbl_Report_Images");

            migrationBuilder.DropTable(
                name: "tbl_User_Badges");

            migrationBuilder.DropTable(
                name: "tbl_User_Rewards");

            migrationBuilder.DropTable(
                name: "tbl_Reports");

            migrationBuilder.DropTable(
                name: "tbl_Badges");

            migrationBuilder.DropTable(
                name: "tbl_Rewards");

            migrationBuilder.DropTable(
                name: "tbl_Categories");

            migrationBuilder.DropTable(
                name: "tbl_Departments");

            migrationBuilder.DropTable(
                name: "tbl_Users");

            migrationBuilder.DropTable(
                name: "tbl_Roles");
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace W10Praktikum
{
    public partial class Form1 : Form
    {
        public static string SqlConnection = "server = localhost; uid = root; pwd =; database = premier_league";
        public MySqlConnection sqlConnect = new MySqlConnection(SqlConnection);
        public MySqlCommand sqlCommand;
        public MySqlDataAdapter sqlAdapter;
        public string sqlQuery;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DataTable dataTeamKiri = new DataTable();
            sqlQuery = "SELECT team_name as 'Team Name', team_id as 'ID Team' FROM team";
            sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(dataTeamKiri);
            ComboBoxKiri.DisplayMember = "Team Name";
            ComboBoxKiri.ValueMember = "ID Team";
            ComboBoxKiri.DataSource = dataTeamKiri;

            DataTable dataTeamKanan = new DataTable();
            sqlQuery = "SELECT team_name as 'Team Name', team_id as 'ID Team' FROM team";
            sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(dataTeamKanan);
            ComboBoxKanan.DisplayMember = "Team Name";
            ComboBoxKanan.ValueMember = "ID Team";
            ComboBoxKanan.DataSource = dataTeamKanan;
        }
        
        private void ComboBoxKiri_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable dataManagerDanCaptainKiri = new DataTable();
            sqlQuery = "SELECT manager.manager_name as `Manager Name`, player.player_name as `Captain Name` FROM team, player, manager WHERE team.captain_id = player.player_id AND manager.manager_id = team.manager_id AND team.team_id = '" + ComboBoxKiri.SelectedValue.ToString() + "'";
            sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(dataManagerDanCaptainKiri);
            OutputManagerKiri.Text = dataManagerDanCaptainKiri.Rows[0]["Manager Name"].ToString();
            OutputKaptenKiri.Text = dataManagerDanCaptainKiri.Rows[0]["Captain Name"].ToString();
        }

        private void ComboBoxKanan_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable dataManagerDanCaptainKanan = new DataTable();
            sqlQuery = "SELECT manager.manager_name as `Manager Name`, player.player_name as `Captain Name` FROM team, player, manager WHERE team.captain_id = player.player_id AND manager.manager_id = team.manager_id AND team.team_id = '" + ComboBoxKanan.SelectedValue.ToString() + "'";
            sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(dataManagerDanCaptainKanan);
            OutputManagerKanan.Text = dataManagerDanCaptainKanan.Rows[0]["Manager Name"].ToString();
            OutputKaptenKanan.Text = dataManagerDanCaptainKanan.Rows[0]["Captain Name"].ToString();

            DataTable Stadium = new DataTable();
            sqlQuery = "SELECT concat(home_stadium, ', ',team.city) as `Stadium`, capacity as `Capacity` FROM team WHERE team_id = '" + ComboBoxKiri.SelectedValue.ToString() + "'";
            sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(Stadium);
            OutputStadium.Text = Stadium.Rows[0]["Stadium"].ToString();
            OutputCapacity.Text = Stadium.Rows[0]["Capacity"].ToString();
        }

        private void buttonDetailMatch_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable TanggalDanSKor = new DataTable();
                sqlQuery = "select date_format(match_date, '%e %M %Y') as 'Tanggal', concat(goal_home, '-', goal_away) as 'Skor' from `match` where team_home = '"+ComboBoxKiri.SelectedValue.ToString()+"' and team_away = '"+ComboBoxKanan.SelectedValue.ToString()+"'";
                sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
                sqlAdapter = new MySqlDataAdapter(sqlCommand);
                sqlAdapter.Fill(TanggalDanSKor);
                labelOutputTanggal.Text = TanggalDanSKor.Rows[0]["Tanggal"].ToString();
                labelOutputSkor.Text = TanggalDanSKor.Rows[0]["Skor"].ToString();
            }
            catch (Exception)
            {
                MessageBox.Show("Pertandingan antara kedua tim tidak ada!");                
            }

            DataTable DetailMatch = new DataTable();
            sqlQuery = "select d1.`minute` as 'Minute', if(d1.team_id = m1.team_home, h.player_name, '') as 'Player Name 1', if(d1.`type`='CY', 'Yellow Card', if(d1.`type`='CR', 'Red Card', if(d1.`type`='GO', 'Goal', if(d1.`type`='GP', 'Goal Penalty', if(d1.`type`='GW', 'Own Goal', if(d1.`type`='PM', 'Penalty Miss', 0)))))) as 'Tipe 1', " +
                "if (d2.team_id = m1.team_away, a.player_name, '') as 'Player Name 2', if (d1.`type`= 'CY', 'Yellow Card', if (d1.`type`= 'CR', 'Red Card', if (d1.`type`= 'GO', 'Goal', if (d1.`type`= 'GP', 'Goal Penalty', if (d1.`type`= 'GW', 'Own Goal', if (d1.`type`= 'PM', 'Penalty Miss', 0)))))) as 'Tipe 2' " +
                "from dmatch d1, player h, `match` m1, dmatch d2, player a, `match` m2 " +
                "where d1.match_id = m1.match_id and h.player_id = d1.player_id and d2.match_id = m2.match_id and a.player_id = d2.player_id and m1.team_home = '"+ComboBoxKiri.SelectedValue.ToString()+"' and m1.team_away = '"+ComboBoxKanan.SelectedValue.ToString()+"' group by d1.minute; ";
            sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(DetailMatch);
            DgvDetailMatch.DataSource = DetailMatch;

        }
    }
}

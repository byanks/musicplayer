using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using WMPLib;
using CsvHelper;
using System.Text.RegularExpressions;


namespace MusicPlayer
{
    public partial class MusicPlayerForm : Form
    {
        public MusicPlayerForm()
        {
            InitializeComponent();
        }

        //Data structure
        private LinkedList<string> SongList = new LinkedList<string>();

        //Add Songs
        private void BtnAddSong_Click(object sender, EventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog //getting the music file
            {
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string file in openFileDialog.FileNames) //add the music file in the list
                {
                    SongList.AddLast(file);
                    ShowSongList();
                }
            }
        }

        //Show Song List
        private void ShowSongList()
        {
            listBoxPlayList.Items.Clear();
            foreach (string file in SongList)
            {
                listBoxPlayList.Items.Add(file);
            }
        }

        //Clear Song List
        private void ClearSongList()
        {
            listBoxPlayList.Items.Clear();
            SongList.Clear();
            labelDisplayCurSong.Text = "";
        }

        //Play Song
        private void PlaySong(string song)
        {
            axWindowsMediaPlayer1.URL = song;
            axWindowsMediaPlayer1.Ctlcontrols.play();
        }

        //Display Current Song
        private void DisplayCurrentSong(string currentSong)
        {
            FileInfo songInfo = new FileInfo(currentSong);
            string songTitle = songInfo.Name;
            labelDisplayCurSong.Text = songTitle;
        }

        //First button
        private void ButtonFirst_Click(object sender, EventArgs e)
        {
            try
            {
                string firstSong = SongList.First();
                PlaySong(firstSong);
                DisplayCurrentSong(firstSong);
                CurrentSongHighlight(firstSong);
            }
            catch
            {
                MessageBox.Show("Song could not be found!");
            }
        }
        
        //Last button
        private void ButtonLast_Click(object sender, EventArgs e)
        {
            try
            {
                string lastSong = SongList.Last();
                PlaySong(lastSong);
                DisplayCurrentSong(lastSong);
                CurrentSongHighlight(lastSong);
            }
            catch
            {
                MessageBox.Show("Song could not be found!");
            }
        }

        //Next button
        private void ButtonNext_Click(object sender, EventArgs e)
        {
            try
            {
                string nextSong = SongList.Find(axWindowsMediaPlayer1.URL).Next.Value;
                DisplayCurrentSong(nextSong);
                PlaySong(nextSong);
                CurrentSongHighlight(nextSong);
            }
            catch
            {
                MessageBox.Show("Song could not be found!");
            }
        }
        
        //Prev button
        private void ButtonPrev_Click(object sender, EventArgs e)
        {
            try
            {
                string prevSong = SongList.Find(axWindowsMediaPlayer1.URL).Previous.Value;
                DisplayCurrentSong(prevSong);
                PlaySong(prevSong);
                CurrentSongHighlight(prevSong);
            }          
            catch
            {
                MessageBox.Show("Song could not be found!");
            }
        }

        //Highlighting current song
        private void CurrentSongHighlight(string song)
        {
            string curSong = SongList.Find(axWindowsMediaPlayer1.URL).Value;
            int indx = listBoxPlayList.FindString(curSong);
            listBoxPlayList.SetSelected(indx, true);
        }

        // Play button
        private void ButtonPlay_Click(object sender, EventArgs e)
        {
            if (listBoxPlayList.SelectedIndex == -1)
            {
                MessageBox.Show("Please select the song!", "Error");
            }
            else
            {
                string curSong = listBoxPlayList.SelectedItem.ToString();
                int indx = listBoxPlayList.FindString(curSong);
                listBoxPlayList.SetSelected(indx, true);
                PlaySong(curSong);
                DisplayCurrentSong(curSong);
            }
        }

        //Stop button
        private void ButtonStop_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.Ctlcontrols.stop();
        }

        //Export List to CSV file
        private void ButtonExport_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "CSV|*.csv", ValidateNames = true }) //getting CSV filename
                {
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        //Save datagridview to csv file
                        using (var writer = new StreamWriter(sfd.FileName)) //open the file in write mode
                        {
                            using (var csv = new CsvWriter(writer, System.Globalization.CultureInfo.CurrentCulture)) //use 3rd library CSV writer
                            {
                                foreach (var item in listBoxPlayList.Items) //add all items in the list box to CSV
                                {
                                    csv.WriteField(item);
                                    csv.NextRecord();
                                }
                            }
                        }
                        MessageBox.Show("Song List has been successfully exported in a CSV file.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        //Import List from CSV file
        private void ButtonImport_Click_1(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog() { Filter = "CSV|*.csv", ValidateNames = true }) //getting CSV filename
                {
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        using (var filereader = new StreamReader(new FileStream(ofd.FileName, FileMode.Open))) //open CSV file
                        {
                            using (var csv = new CsvReader(filereader, System.Globalization.CultureInfo.CurrentCulture)) //use 3rd library CSV reader
                            {
                                string value;
                                csv.Configuration.HasHeaderRecord = false;
                                while (csv.Read()) //read all the lines in the CSV file
                                {
                                    for (int i = 0; csv.TryGetField<string>(i, out value); i++) //add the lines to the list
                                    {
                                        SongList.AddLast(value);
                                    }
                                }
                            }
                        }
                        ShowSongList();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        //Clear List button
        private void buttonClear_Click(object sender, EventArgs e)
        {
            ClearSongList();
        }

        //QuickSort Algorithm method
        private void quicksort(string[] arr, int start, int end)
        {
            if (start < end)
            {
                int pivotIndex = partition(arr, start, end);
                quicksort(arr, start, pivotIndex - 1);
                quicksort(arr, pivotIndex + 1, end);
            }
        }
        private int partition(string[] arr, int start, int end)
        {
            int pivot = end;
            int i = start, j = end;
            string temp;
            while (i < j)
            {
                while (i < end && string.Compare(arr[i], arr[pivot]) < 0)
                    i++;
                while (j > start && string.Compare(arr[j], arr[pivot]) > 0)
                    j--;

                if (i < j)
                {
                    temp = arr[i];
                    arr[i] = arr[j];
                    arr[j] = temp;
                }
            }
            temp = arr[pivot];
            arr[pivot] = arr[j];
            arr[j] = temp;
            return j;
        }
        
        //Sort List method
        private void SortList(string[] array)
        {
            quicksort(array, 0, array.Length - 1); //sorting array alphabetically
            listBoxPlayList.Items.Clear();
            SongList.Clear();
            foreach (string s in array) //adding array back to the list
            {
                SongList.AddLast(s);
                listBoxPlayList.Items.Add(s); //display sorted list
            }
        }

        //Sort button
        private void buttonSort_Click(object sender, EventArgs e)
        {
            string[] myArray = listBoxPlayList.Items.OfType<string>().ToArray(); //add list items to an array
            SortList(myArray);
        }

        //Search song from filtered list 
        private void buttonSearch1_Click(object sender, EventArgs e)
        {
            string[] myArray = listBoxPlayList.Items.OfType<string>().ToArray(); //add list items to an array
            SortList(myArray); //sort array
            
            if (listBoxFoundSongs.SelectedIndex == -1)
            {
                MessageBox.Show("Please select the song!", "Error");
            }
            else
            {
                string userInput = listBoxFoundSongs.SelectedItem.ToString(); 
                int search = SongSearch(myArray, userInput); //Run binary search
                if (search != -1)
                {
                    string curSong = userInput;
                    int indx = listBoxPlayList.FindString(curSong);
                    listBoxPlayList.SetSelected(indx, true); //highlight matched song
                }
                else
                {
                    MessageBox.Show("No song found!");
                }
            }
        }

        //Binary Search method
        static int SongSearch(string [] array, string inputData)
        {
            int first = 0;
            int last = array.Length - 1;
            
            while (first <= last)
            {
                int middle = (first + last) / 2;
                if ((string.Compare(inputData, array.ElementAt(middle).ToString(), StringComparison.OrdinalIgnoreCase)) == 0)  
                {
                    return middle;
                }
                else if ((string.Compare(inputData, array.ElementAt(middle).ToString(), StringComparison.OrdinalIgnoreCase)) > 0) 
                {
                    first = middle + 1;
                }
                else
                {
                    last = middle - 1;
                }
            }
            return -1;
        }

        //Clear button
        private void buttonClear2_Click(object sender, EventArgs e)
        {
            filterListBox.Text = "";
            searchSongBox.Text = "";
            listBoxFoundSongs.Items.Clear();
        }

        //Find partial match using keyword
        private void buttonFind_Click(object sender, EventArgs e)
        {
            string keyword = filterListBox.Text;
            if (String.IsNullOrEmpty(filterListBox.Text))
            {
                MessageBox.Show("Please enter keyword!", "Error");
            }
            else
            {
                listBoxFoundSongs.Items.Clear();
                labelFilteredSong.Text = "Song(s) found with keyword '" + keyword + "'";
                foreach (var s in SongList.Where(s => s.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) > -1))
                {
                    listBoxFoundSongs.Items.Add(s);
                }
            }
        }

        //Search song from URL
        private void buttonSearch2_Click(object sender, EventArgs e)
        {
            string[] myArray = listBoxPlayList.Items.OfType<string>().ToArray(); //add list items to an array
            SortList(myArray); //sort array

            string userInput = searchSongBox.Text;
            int search = SongSearch(myArray, userInput); //Run binary search

            if (search != -1)
            {
                string curSong = userInput;
                int indx = listBoxPlayList.FindString(curSong);
                listBoxPlayList.SetSelected(indx, true); //highlight matched song
            }
            else
            {
                MessageBox.Show("No song found!");
            }
        }
    }
}

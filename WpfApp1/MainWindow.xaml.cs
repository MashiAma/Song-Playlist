using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using NAudio.Wave;


namespace PlaylistX
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private CircularLinkedList<Song> playlist;
        private CircularLinkedList<Song>.CircularLinkedListNode currentSongNode;
        private DispatcherTimer timer;
        private bool subButtonsVisible1 = false;
        private bool subButtonsVisible2 = false;
        private TimeSpan stopTime;
        private bool isPlaying = false;
        private MediaPlayer mediaPlayer;

        public MainWindow()
        {
            InitializeComponent();
            playlist = new CircularLinkedList<Song>();
            InitializePlaylist();
            showHeaderMessage();
            mediaPlayer = new MediaPlayer();
            mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1000);
            timer.Tick += Timer_Tick;
            timer.Start();

        }
         //= new MediaPlayer();
        private bool isAscendingOrder;
        private bool isDescendingOrder;

        private void MediaPlayer_MediaEnded(object? sender, EventArgs e)
        {
            if (currentSongNode != null)
            {
                currentSongNode = currentSongNode.Next;
                PlaySelectedSong();
            }
        }
        //private void PlayMedia()
        //{
        //    if (currentMedia != null)
        //    {
        //        mediaElement.Source = new Uri(currentMedia.Value, UriKind.Relative);
        //        mediaElement.Play();
        //    }

        //}



        private void showHeaderMessage()
        {
            DateTime currentTime = DateTime.Now;
            TimeSpan currentDayTime = currentTime.TimeOfDay;

            if (currentDayTime < new TimeSpan(12, 0, 0))
            {
                lbl_message.Content = "Good Morning, Amashi";
            }
            else
            {
                lbl_message.Content = "Good Evening, Amashi";
            }
        }
        private void InitializePlaylist()
        {
            //Thses Songs are already added 
            Song song1 = new Song("2002", "D:\\Entertainment\\Songs\\2002.mp3", "Anne Marie", "00:00", "03:07");
            Song song2 = new Song("I Ain't Worried", "D:\\Entertainment\\Songs\\I Ain't Worried.mp3", "One Republic", "00:00", "02:27");
            Song song3 = new Song("Our Song", "D:\\Entertainment\\Songs\\OurSong.mp3", "Anne Marie/Niall Horan", "00:00", "02:43");

            playlist.AddLast(song1);
            playlist.AddLast(song2);
            playlist.AddLast(song3);
            listBox.ItemsSource = playlist.ToArray();
        }
        //if (playlist.Count>0)
        //{
        //    playlist.MovePrevious();
        //    string selectedSong = playlist.GetCurrent();
        //    listBox.SelectedItem = selectedSong;
        //    //PlayMedia();
        //}
        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
           
            if (currentSongNode != null)
            {
                
                if(isDescendingOrder==true)
                {
                    // Descending order
                        currentSongNode = currentSongNode.Next;
                        PlaySelectedSong();
                    
                }
                else if (isAscendingOrder==true)
                {
                        currentSongNode = currentSongNode.Previous;
                        PlaySelectedSong();
                }
                else
                {
                        currentSongNode = currentSongNode.Previous;
                        PlaySelectedSong();
 
                }
            }
        }
        private void PlaySelectedSong()
        {
            if (currentSongNode != null)
            {
                Song selectedSong = currentSongNode.Value;
                listBox.SelectedItem = selectedSong;
                mediaPlayer.Open(new Uri(selectedSong.FilePath));
                mediaPlayer.Play();
                UpdateCurrentlyPlayingLabels();
            }

        }
        //if (playlist.Count >0)
        //{
        //    playlist.MoveNext();
        //    string selectedSong = playlist.GetCurrent();
        //    listBox.SelectedItem = selectedSong;
        //    //PlayMedia();
        //}
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {

           

            if (currentSongNode != null)
            {

                if (isDescendingOrder == true)
                {
                    // Descending order
                    currentSongNode = currentSongNode.Previous;
                    PlaySelectedSong();

                }
                else if (isAscendingOrder == true)
                {
                    currentSongNode = currentSongNode.Next;
                    PlaySelectedSong();
                }
                else
                {
                    currentSongNode = currentSongNode.Next;
                    PlaySelectedSong();
                }
            }
            
        }

        private void lbl_close__MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btn_add_song_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "MP3 Files (*.mp3)|*.mp3|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
                string artist = "Unknown";
                string sduration = "00:00";
                string duration = GetSongDuration(filePath);
                // Add the song 
                Song newSong = new Song(fileName, filePath, artist, sduration, duration);
                playlist.AddLast(newSong);
                listBox.ItemsSource = playlist.ToArray();

            }
        }
   
        private void btn_play_Click(object sender, RoutedEventArgs e)
        {
            Song selectedSong = listBox.SelectedItem as Song;
            if (selectedSong != null)
            {
                if (mediaPlayer.Source != null && mediaPlayer.Source.LocalPath == selectedSong.FilePath)
                {
                    if (isPlaying)
                    {
                        mediaPlayer.Pause();
                        timer.Stop();
                        stopTime = mediaPlayer.Position;
                        isPlaying = false;
                    }
                    else
                    {
                        mediaPlayer.Play();
                        timer.Start();
                        isPlaying = true;
                    }
                }
                else
                {
                    mediaPlayer.Open(new Uri(selectedSong.FilePath));
                    mediaPlayer.Play();
                    timer.Start();
                    stopTime = TimeSpan.Zero;
                    isPlaying = true;
                }
                currentSongNode = playlist.Find(selectedSong);
                UpdateCurrentlyPlayingLabels();
            }

        }
        //private void Stop_Click(object sender, RoutedEventArgs e)
        //{
        //    mediaPlayer.Stop();
        //    UpdateCurrentlyPlayingLabels();
        //}

        private void UpdateCurrentlyPlayingLabels()
        {
            if (currentSongNode != null)
            {
                Song selectedSong = currentSongNode.Value;
                lbl_songName.Content = selectedSong.Name;
                lbl_songArtist.Content = selectedSong.Artist;
                if (isPlaying)
                {
                lbl_sduration.Content = mediaPlayer.Position.ToString(@"mm\:ss");
                }
                else
                {
                lbl_sduration.Content = stopTime.ToString(@"mm\:ss");
        
                }       lbl_endduration.Content = selectedSong.Duration;
            }
            else
            {
                lbl_songName.Content = string.Empty;
                lbl_songArtist.Content = string.Empty;
                lbl_sduration.Content = string.Empty;
                lbl_endduration.Content = string.Empty;
            }
        }
        private string GetSongDuration(string filePath)
        {
            try
            {
                using (var audioFile = new AudioFileReader(filePath))
                {
                    TimeSpan duration = audioFile.TotalTime;
                    return duration.ToString(@"mm\:ss");
                }
            }
            catch (Exception ex)
            {
                // Exception for no time
                Console.WriteLine("Error");
            }

            return string.Empty;
        }


        private void slider_song_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (mediaPlayer.NaturalDuration.HasTimeSpan)
            {
                TimeSpan duration = mediaPlayer.NaturalDuration.TimeSpan;
                TimeSpan newPosition = TimeSpan.FromSeconds((e.NewValue / 100) * duration.TotalSeconds);
                mediaPlayer.Position = newPosition;
                lbl_sduration.Content = newPosition.ToString(@"mm\:ss");
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (mediaPlayer.NaturalDuration.HasTimeSpan)
            {
                TimeSpan duration = mediaPlayer.NaturalDuration.TimeSpan;
                slider_song.Value = (mediaPlayer.Position.TotalSeconds / duration.TotalSeconds) * 100;
                lbl_sduration.Content = TimeSpan.FromSeconds((slider_song.Value / 100) * duration.TotalSeconds).ToString(@"mm\:ss");
            }

        }

        public class Song
        {
            public string Name { get; set; }
            public string FilePath { get; set; }
            public string Artist { get; set; }
            public string startTime { get; set; }
            public string Duration { get; set; }

            public Song(string name, string filePath, string artist, string sduration, string duration)
            {
                Name = name;
                FilePath = filePath;
                Artist = artist;
                startTime = sduration;
                Duration = duration;
            }

            public override string ToString()
            {
                return Name;
                return Artist;
                return startTime;
                return Duration;
            }
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mediaPlayer.Source != null && mediaPlayer.Position != TimeSpan.Zero)
                return; // Don't update details while a song is playing

            if (listBox.SelectedItem != null)
            {
                Song selectedSong = listBox.SelectedItem as Song;
                currentSongNode = playlist.Find(selectedSong);
                UpdateCurrentlyPlayingLabels();
            }
        }

        private void btn_remove_song_Click_1(object sender, RoutedEventArgs e)
        {
            Song selectedSong = listBox.SelectedItem as Song;
            if (currentSongNode != null && currentSongNode.Value == selectedSong)
            {
                CircularLinkedList<Song>.CircularLinkedListNode nextNode = currentSongNode.Next;
                playlist.Remove(currentSongNode);
                currentSongNode = nextNode;
                if (currentSongNode != null)
                    PlaySelectedSong();
                else
                    mediaPlayer.Stop();
                UpdateCurrentlyPlayingLabels();
                listBox.ItemsSource = playlist.ToArray();
            }
        }

        private void showHiddenButton1_Click(object sender, RoutedEventArgs e)
        {
            subButtonsVisible1 = !subButtonsVisible1;
            if (subButtonsVisible1)
            {
                btn_remove_song.Visibility = Visibility.Visible;
                btn_romove_Firstlist.Visibility = Visibility.Visible;
                btn_romove_Lastlist.Visibility = Visibility.Visible;
            }
            else
            {
                btn_remove_song.Visibility = Visibility.Collapsed;
                btn_romove_Firstlist.Visibility = Visibility.Collapsed;
                btn_romove_Lastlist.Visibility = Visibility.Collapsed;
            }
        }

        private void showHiddenButton2_Click(object sender, RoutedEventArgs e)
        {
            subButtonsVisible2 = !subButtonsVisible2;
            if (subButtonsVisible2)
            {
                btn_ascending.Visibility = Visibility.Visible;
                btn_descending.Visibility = Visibility.Visible;
            }
            else
            {
                btn_ascending.Visibility = Visibility.Collapsed;
                btn_descending.Visibility = Visibility.Collapsed;
            }
        }

        private void btn_romove_Lastlist_Click(object sender, RoutedEventArgs e)
        {
            if (currentSongNode != null)
            {
                CircularLinkedList<Song>.CircularLinkedListNode removedNode = playlist.tail;

                if (currentSongNode == removedNode)
                {
                    currentSongNode = removedNode.Previous;
                    PlaySelectedSong();
                }

                playlist.Remove(removedNode);
                listBox.ItemsSource = playlist.ToArray();
            }
            //CircularLinkedList<Song>.CircularLinkedListNode removedNode = currentSongNode;

            //    if (currentSongNode.Next != currentSongNode)
            //    {
            //        currentSongNode = currentSongNode.Next;
            //        PlaySelectedSong();
            //    }
            //    else
            //    {
            //        currentSongNode = null;
            //        mediaPlayer.Stop();
            //        UpdateCurrentlyPlayingLabels();
            //    }

            //    playlist.Remove(removedNode);
            //    listBox.ItemsSource = playlist.ToArray();
            //}
        }

        private void btn_romove_Firstlist_Click(object sender, RoutedEventArgs e)
        {
            if (currentSongNode != null)
            {
                CircularLinkedList<Song>.CircularLinkedListNode removedNode = playlist.head;

                if (currentSongNode == removedNode)
                {
                    currentSongNode = removedNode.Next;
                    PlaySelectedSong();
                }

                playlist.Remove(removedNode);
                listBox.ItemsSource = playlist.ToArray();
                //CircularLinkedList<Song>.CircularLinkedListNode removedNode = currentSongNode;

                //if (currentSongNode.Next != currentSongNode)
                //{
                //    currentSongNode = currentSongNode.Previous;
                //    PlaySelectedSong();
                //}
                //else
                //{
                //    currentSongNode = null;
                //    mediaPlayer.Stop();
                //    UpdateCurrentlyPlayingLabels();
                //}

                //playlist.Remove(removedNode);
                //listBox.ItemsSource = playlist.ToArray();
            }
        }
        private bool IsAscendingOrder()
        {
            List<Song> playlistItems = listBox.Items.Cast<Song>().ToList();

            for (int i = 0; i < playlistItems.Count - 1; i++)
            {
                int minIndex = i;
                for (int j = i + 1; j < playlistItems.Count; j++)
                {
                    if (string.Compare(playlistItems[j].Name, playlistItems[minIndex].Name, StringComparison.Ordinal) < 0)
                    {
                        minIndex = j;
                    }
                }

                if (minIndex != i)
                {
                    Song temp = playlistItems[i];
                    playlistItems[i] = playlistItems[minIndex];
                    playlistItems[minIndex] = temp;
                }
            }

            listBox.ItemsSource = playlistItems;
            
            return true;
        }
        private void btn_ascending_Click(object sender, RoutedEventArgs e)
        {

            IsAscendingOrder();
            isAscendingOrder = true;

        }
        private bool IsDescendingOrder()
        {
            List<Song> playlistItems = listBox.Items.Cast<Song>().ToList();

            for (int i = 0; i < playlistItems.Count - 1; i++)
            {
                for (int j = 0; j < playlistItems.Count - i - 1; j++)
                {
                    if (string.Compare(playlistItems[j].Name, playlistItems[j + 1].Name, StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        Song temp = playlistItems[j];
                        playlistItems[j] = playlistItems[j + 1];
                        playlistItems[j + 1] = temp;
                    }
                }
            }
            listBox.ItemsSource = playlistItems;
            return true;
        }

        private void btn_descending_Click(object sender, RoutedEventArgs e)
        {
            IsDescendingOrder();
            isDescendingOrder= true;
        }
        public class CircularLinkedList<T>
        {
            public class CircularLinkedListNode
            {
                public T Value { get; }
                public CircularLinkedListNode Next { get; set; }
                public CircularLinkedListNode Previous { get; set; }

                public CircularLinkedListNode(T value)
                {
                    Value = value;
                }
            }

            public CircularLinkedListNode head;
            public CircularLinkedListNode tail;
            //        private int GetSize()
            //        {
            //            if (head == null)
            //                return 0;

            //            int count = 0;
            //            Node current = head;

            //            do
            //            {
            //                count++;
            //                current = current.next;
            //            } while (current != head);

            //            return count;
            //        }
            //        public T GetCurrent()
            //        {
            //            if (current != null)
            //                return current.Value;
            //            else
            //                return default(T);
            //        }

            //        public void MoveNext()
            //        {
            //            if (current != null)
            //                current = current.next;
            //        }
            //        public void MovePrevious()
            //        {
            //            if (current != null)
            //            {
            //                Node previous = head;
            //                while (previous.next != current)
            //                    previous = previous.next;
            //                current = previous;
            //            }

            //        }
            //    }
            //}

            public void AddLast(T value)
            {
                CircularLinkedListNode newNode = new CircularLinkedListNode(value);

                if (head == null)
                {
                    head = newNode;
                    tail = newNode;
                    head.Next = tail;
                    head.Previous = tail;
                    tail.Next = head;
                    tail.Previous = head;
                }
                else
                {
                    newNode.Previous = tail;
                    newNode.Next = head;
                    tail.Next = newNode;
                    head.Previous = newNode;
                    tail = newNode;
                }
            }
            public void Remove(CircularLinkedListNode node)
            {
                if (node == head && node == tail)
                {
                    head = null;
                    tail = null;
                }
                else
                {
                    node.Previous.Next = node.Next;
                    node.Next.Previous = node.Previous;
                    if (node == head)
                        head = node.Next;
                    if (node == tail)
                        tail = node.Previous;
                }
            }
            public CircularLinkedListNode Find(T value)
            {
                if (head == null)
                    return null;

                CircularLinkedListNode node = head;
                do
                {
                    if (node.Value.Equals(value))
                        return node;

                    node = node.Next;
                } while (node != head);

                return null;
            }

            public T[] ToArray()
            {
                if (head == null)
                    return new T[0];

                List<T> list = new List<T>();
                CircularLinkedListNode node = head;
                do
                {
                    list.Add(node.Value);
                    node = node.Next;
                } while (node != head);

                return list.ToArray();
            }
        }
    }
}


//private void Updateslider_song()
//{
//    if (currentSongNode != null)
//    {
//        Song selectedSong = currentSongNode.Value;
//        if (mediaPlayer.NaturalDuration.HasTimeSpan)
//        {
//            TimeSpan duration = mediaPlayer.NaturalDuration.TimeSpan;
//            slider_song.Maximum = 100;
//            slider_song.Value = (mediaPlayer.Position.TotalSeconds / duration.TotalSeconds) * 100;
//        }
//        else
//        {
//            slider_song.Maximum = 0;
//            slider_song.Value = 0;
//        }
//    }
//    else
//    {
//        slider_song.Maximum = 0;
//        slider_song.Value = 0;
//    }
//}


//private double CalculateSongPosition(double sliderValue)
//{/*9
//    if (currentSongNode != null)
//    {
//        double duration = currentSongNode.Value.duration;
//        double songPosition = (sliderValue / 100) * duration;
//        return songPosition;
//    }

//    return 0;
//}
//private void UpdateCurrentSongPosition(double songPosition)
//{

//    // Example:
//    if (currentSongNode != null)
//    {
//        currentSongNode.Value.Position = songPosition;
//    }



//private string GetFilePath(string songName)
//{
//    return $"D:\\Entertainment\\Songs\\{songName}";
//}

//private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
//{
//    string selectedSong = listBox.SelectedItem as string;
//    if (selectedSong != null)
//    {
//        currentSongNode = playlist.Find(selectedSong);
//    }
//}



//    private class Node
//        {
//            public T Value { get; set; }
//            public Node next { get; set; }
//        }
//        private Node head;
//        private Node tail;
//        private Node current;
//        private int count;

//        public int Count => count;

//        public void AddTrack(T value)
//        {
//            Node newNode = new Node { Value = value };

//            if (head == null)
//            {
//                head = newNode;
//                tail = newNode;
//                tail.next = head;
//            }
//            else
//            {
//                tail.next = newNode;
//                tail = newNode;
//                tail.next = head;

//            }

//        }

///Realted to change state slider

//{
//    if (mediaPlayer.NaturalDuration.HasTimeSpan)
//    {
//        TimeSpan duration = mediaPlayer.NaturalDuration.TimeSpan;
//        TimeSpan newPosition = TimeSpan.FromSeconds((e.NewValue / 100) * duration.TotalSeconds);
//        mediaPlayer.Position = newPosition;
//    }



//realated to time ick



//sliderValue = slider_song.Value;

//// Calculate the corresponding position in the song
//double songPosition = CalculateSongPosition(sliderValue);

//// Set the current song position
//UpdateCurrentSongPosition(songPosition);


//if (mediaPlayer.NaturalDuration.HasTimeSpan && mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds > 0)
//{
//    TimeSpan duration = mediaPlayer.NaturalDuration.TimeSpan;
//    TimeSpan currentPosition = mediaPlayer.Position;
//    double progress = (currentPosition.TotalSeconds / duration.TotalSeconds) * 100;
//    slider_song.Value = progress;

//    Song selectedSong = currentSongNode?.Value;
//    if (selectedSong != null)
//    {
//        TimeSpan startTime = currentPosition.Subtract(duration);
//        selectedSong.startTime = startTime.ToString(@"mm\:ss");
//        UpdateCurrentlyPlayingLabels();
//    }
//}
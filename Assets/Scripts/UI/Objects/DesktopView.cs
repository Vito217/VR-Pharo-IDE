using System.Collections;
using System.Net.Http;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DesktopView : InitializeBehaviour
{
    public Image img;
    public static string streamerIP;

    private bool click = false;
    private bool keepRequesting = true;
    private Vector2 coords = Vector2.zero;

    private HttpClient streamerClient;

    public override IEnumerator innerStart()
    {
        streamerClient = new HttpClient();

        ReadFromURL();
        return base.innerStart();
    }

    public void OnPointerClick(BaseEventData data)
    {
        click = true;
        Vector3 hitWorldPosition = ((PointerEventData)data).pointerCurrentRaycast.worldPosition;
        Vector2 hitLocalPosition = img.transform.InverseTransformPoint(hitWorldPosition);
        Vector2 delta = GetComponent<RectTransform>().sizeDelta / 2;
        coords = Vector2Int.RoundToInt(new Vector2(hitLocalPosition.x + delta.x, delta.y - hitLocalPosition.y));
    }

    async void ReadFromURL()
    {
        while (keepRequesting)
        {
            try
            {
                // Building the data
                string data = click + " " + coords.x + " " + coords.y;

                if (click) click = false;

                HttpContent content = new ByteArrayContent(Encoding.UTF8.GetBytes(data));
                HttpResponseMessage response = await streamerClient.PostAsync(streamerIP, content);
                byte[] result = await response.Content.ReadAsByteArrayAsync();

                Texture2D tex = new Texture2D(1, 1);
                tex.LoadImage(result);

                float width = tex.width;
                float height = tex.height;

                //Updating window size
                GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);

                // Updating image
                img.sprite = Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(width * .5f, height * .5f));
            }
            catch { }
        }
    }

    private void OnDestroy()
    {
        keepRequesting = false;
        streamerClient.Dispose();
    }
}

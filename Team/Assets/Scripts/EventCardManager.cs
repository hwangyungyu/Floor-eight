using System.Collections.Generic;
using UnityEngine;

public class EventCardManager : MonoBehaviour
{
    private static List<EventCardDeck> eventCardDeckList = new List<EventCardDeck>(); //�� ����Ʈ���� 0�� �ε����� ������� �ʽ��ϴ�. 1������ 1�� �ε����� �������ּ���.

    private EventCard currentEventCard; // ���� ȭ�鿡 ǥ�� ���� �̺�Ʈ ī��
    public EventCard CurrentEventCard => currentEventCard;

    private int currentCardDay = 1;  //DrawEventCard()���� ���� ī�尡 ��� ������ ���Դ����� �����ϴ� ������
    private int currentCardIndex = -1;  //�ӽ÷� 1���� -1���� ���� ī��� �����߽��ϴ�. DrawEventCard�� ���� 1���� 0���� �����ɴϴ�.

    // EventCardDeck���� ���� ī�带 ������
    public void DrawEventCard()
    {
        // ���� ��¥�� ��ȿ���� �ʴٸ� ����
        if (!IsValidDay(currentCardDay)) return;

        EventCardDeck currentDeck = eventCardDeckList[currentCardDay];

        // ���� ��¥�� ī�� ���� ��������� ��� ��� �� ����
        if (currentDeck.EventCardCount == 0)
        {
            Debug.LogWarning("���� ��¥�� ī�� ���� ��� �ֽ��ϴ�.");
            return;
        }

        // ���� ī�尡 ������ ī���� ������ ù��° ī�� �ҷ�����(�� �ڵ尡 �������� �Ѿ�� �ص� �Ǵ���?)
        if (currentCardIndex >= currentDeck.EventCardCount - 1)
        {
            currentCardDay++;
            currentCardIndex = -1;

            // ���� ���� ��ȿ���� Ȯ��
            if (!IsValidDay(currentCardDay))
            {
                Debug.LogWarning("�� �̻� ������ �̺�Ʈ ī�尡 �����ϴ�.");
                return;
            }

            // ���� ���� ���� �ٽ� ����
            if (currentCardDay < eventCardDeckList.Count)
            {
                currentDeck = eventCardDeckList[currentCardDay];
            }
            else
            {
                Debug.LogWarning("��� �̺�Ʈ ī�带 �����߽��ϴ�.");
                return;
            }
        }

        // Index���� ������Ʈ �� ī�� �����
        currentCardIndex++;
        currentEventCard = currentDeck.GetEventCard(currentCardIndex);
    }

    // Ư����¥�� ���� ī�� ����
    public void InsertEventCardToDeck(int day, EventCard eventCard, int index)
    {
        if (IsValidDay(day))
        {
            eventCardDeckList[day].InsertEventCard(eventCard, index);
        }
    }

    // Ư�� ��¥(day)�� ī�� ���� �̺�Ʈ ī�带 �߰��� �� ����
    public void AddEventCardWithShuffle(int day, EventCard eventCard)
    {
        if (IsValidDay(day))
        {
            eventCardDeckList[day].AddEventCard(eventCard);
            eventCardDeckList[day].ShuffleDeck();
        }
    }

    // Ư�� ��¥(day)�� ī�� ������ ������ ��ġ(index)�� �̺�Ʈ ī�带 ����
    public void RemoveEventCard(int day, int index)
    {
        if (IsValidDay(day))
        {
            eventCardDeckList[day].RemoveEventCard(index);
        }
    }

    // ��ȿ�� ��¥(day)���� Ȯ���ϴ� �Լ�
    private bool IsValidDay(int day)
    {
        return day >= 0 && day < eventCardDeckList.Count;
    }
}

public class EventCardDeck
{
    private List<EventCard> eventCardList = new List<EventCard>();

    public int EventCardCount => eventCardList.Count;  //ī�� ������ �ľ��ϱ� ���� �߰�

    public EventCard GetEventCard(int index)
    {
        if (index < 0 || index > eventCardList.Count)
        {
            Debug.LogWarning("�߸��� �ε��� ���Դϴ�.");
            return null;
        }
        return eventCardList[index];
    }

    //�� ����
    public void ShuffleDeck()
    {
        for (int i = eventCardList.Count - 1; i > 0; i--)       
        {
            int randomIndex = Random.Range(0, i + 1);
            (eventCardList[i], eventCardList[randomIndex]) = (eventCardList[randomIndex], eventCardList[i]);
        }
    }

    //���� �������� eventCard�� �߰�
    public void AddEventCard(EventCard eventCard)
    {
        eventCardList.Add(eventCard);
    }

    //�ε����� eventCard ����
    public void InsertEventCard(EventCard eventCard, int index)
    {
        if (index < 0 || index > eventCardList.Count)
        {
            Debug.LogWarning("�߸��� �ε��� ���Դϴ�.");
            return;
        }
        eventCardList.Insert(index, eventCard);
    }

    // ������ �ε����� �̺�Ʈ ī�带 �����ϰ� ��ȯ
    public EventCard RemoveEventCard(int index = -1)
    {
        if (eventCardList.Count == 0)
        {
            Debug.LogWarning("���� ��� �ֽ��ϴ�.");
            return null;
        }

        // �⺻������ ������ ī�带 ����
        if (index < 0 || index >= eventCardList.Count)
        {
            index = eventCardList.Count - 1;
        }

        EventCard removedCard = eventCardList[index];
        eventCardList.RemoveAt(index);
        return removedCard;
    }
}
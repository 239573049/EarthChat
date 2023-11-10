using System.Text;

namespace Chat.Service.Infrastructure.Helper;

public sealed class SensitiveWordsAc
{
    private static readonly Lazy<SensitiveWordsAc> _instance = new(() => new SensitiveWordsAc());

    private TrieNode _root;
    private Dictionary<TrieNode, TrieNode> _failureLinks;

    public static SensitiveWordsAc Instance => _instance.Value;

    /// <summary>
    /// 构建AC自动机
    /// </summary>
    /// <param name="words"></param>
    public void Build(List<SensitiveWords> words)
    {
        // 构建AC-Trie树
        _root = new TrieNode();
        foreach (var word in words)
        {
            _root.Insert(word.Value);
        }

        // 构建失败链路 BFS
        _failureLinks = new Dictionary<TrieNode, TrieNode>();
        var queue = new Queue<TrieNode>();
        foreach (var child in _root.Children.Values)
        {
            _failureLinks[child] = _root;
            queue.Enqueue(child);
        }

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            foreach (var child in node.Children.Values)
            {
                var failure = _failureLinks[node];
                while (failure != _root && !failure.Children.ContainsKey(child.Value))
                {
                    failure = _failureLinks[failure];
                }

                if (failure.Children.ContainsKey(child.Value))
                {
                    failure = failure.Children[child.Value];
                }

                _failureLinks[child] = failure;
                queue.Enqueue(child);
            }
        }
    }

    /// <summary>
    /// 寻找敏感词
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public List<string> Find(string text)
    {
        var matches = new List<string>();
        var cur = _root;
        var word = new StringBuilder();

        for (int i = 0; i < text.Length; i++)
        {
            var c = text[i];
            while (cur != _root && !cur.Children.ContainsKey(c))
            {
                cur = _failureLinks[cur];
            }

            if (cur.Children.ContainsKey(c))
            {
                cur = cur.Children[c];
                word.Append(c);
                if (cur.IsEndOfWord)
                {
                    matches.Add(word.ToString());
                    word.Clear();
                }
            }
            else
            {
                // 在遇到无法匹配的字符时，将当前节点回溯到Trie根节点；
                cur = _root;
                word.Clear();
            }
        }

        return matches;
    }

    /// <summary>
    /// 字符串脱敏
    /// </summary>
    /// <param name="str"></param>
    /// <param name="c">替换符</param>
    /// <returns></returns>
    public string TakeOutSensitive(string str, char c = '*')
    {
        var sensitiveWords = Instance.Find(str);

        var sb = new StringBuilder(str);
        foreach (var item in sensitiveWords)
        {
            sb.Replace(item, new string(c, item.Length));
        }

        return sb.ToString();
    }
    
    private class TrieNode
    {
        public char Value { get; set; }
        public bool IsEndOfWord { get; set; }
        public Dictionary<char, TrieNode> Children { get; set; }

        public TrieNode()
        {
            Children = new Dictionary<char, TrieNode>();
        }

        public void Insert(string word)
        {
            TrieNode current = this;
            foreach (char c in word)
            {
                if (!current.Children.ContainsKey(c))
                {
                    current.Children[c] = new TrieNode() { Value = c };
                }

                current = current.Children[c];
            }

            current.IsEndOfWord = true;
        }
    }
}

public class SensitiveWords
{
    public string Value { get; set; }
}